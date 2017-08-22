using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Lib_Net;

namespace AutoMarkDCTFile
{
    public partial class fmain : Form
    {
     //   private delegate void updateDelegate(string strTime, string endTime, string product, string tester, string comments, int nMark);

        public fmain()
        {
            InitializeComponent();
        }
        string _configureFile = null;
        DateTime _yarmstamptime;
        string _exeName;

        private void Form1_Load(object sender, EventArgs e)
        {
            _exeName = Application.ExecutablePath.Replace(Application.StartupPath, "").Replace(@"\", "").Replace(".exe", "").Replace(".EXE", "");
            this.Text = _exeName + " V " + ProductVersion;
            _configureFile = Application.StartupPath + @"\" + _exeName + ".ini";
            //Load ini file          
            Configure.SetIniFile = _configureFile;
            string getErro = Configure.LoadConfigure();     
           if (string.IsNullOrEmpty(getErro))
            {
                _yarmstamptime = DateTime.Now.AddMinutes(5);
                timerMonitor.Enabled = true;
            }
            else
            {                                  
                WRLog wrlog = new WRLog();
                wrlog.WriteLogFile(Application.StartupPath, "Error Configure is loading.ini", getErro);
            }

        }
        private void timerMonitor_Tick(object sender, EventArgs e)
        {
            timerMonitor.Enabled = false;
            try
            {
                //Get all paths
                string MonPath = Configure.GetMonitorPath;
                string PreRDDPath = Configure.GetPreRDDPath;
                string EDBPath = Configure.GetEDBPath;
                string RTTCPath = Configure.GetRTTCPath;
                string OCRPath = Configure.GetOCRPath;
                string completePath = Configure.GetCompletedPath;
                string notCompletePath = Configure.GetNotCompletePath;

                string emailaddress = Configure.GetEmailAddress;

                //DirectoryInfo drInfo = new DirectoryInfo(MonPath);
                //FileInfo[] fileInfo = drInfo.GetFiles("*.ini");
                FileInfo[] fileInfo = GetFileInfo(MonPath);

                Boolean chkOpen = Configure.OpenDatabase;  // Open database connection
                if (chkOpen.Equals(false)) { chkOpen = Configure.ReOpenDatabase; }            
                MySql.Data.MySqlClient.MySqlConnection myConn = Configure.GetRTTCStringConnection;
                //start auto mark
                AutoMarkSlider autoMark = new AutoMarkSlider();
                for (int i = 0; i < fileInfo.Length; i++)
                {
                    //-------Refresh
                    RefershInfo();
                    //---------------------
                    string FileName = fileInfo[i].Name;
                    string FileFullName = fileInfo[i].FullName;
                    CGetMemSetting getmarkinfo = new CGetMemSetting(FileFullName);
     
                    autoMark.setProduct = getmarkinfo.GetValueString("Markreject", "Product");
                    autoMark.setTester = getmarkinfo.GetValueString("Markreject", "Tester");
                    autoMark.setStartTime = Convert.ToDateTime(getmarkinfo.GetValueString("Markreject","StartTime"));
                    autoMark.setEndTime = Convert.ToDateTime(getmarkinfo.GetValueString("Markreject", "EndTime"));
                    autoMark.setMarkGrdID = getmarkinfo.GetValueString("Markreject", "GradeID");
                    autoMark.setMarkGrdName = getmarkinfo.GetValueString("Markreject", "GradeName");
                    autoMark.SetAppPath = Application.StartupPath;
                    autoMark.SetNSldSetting = Configure.GetNSlider;  //N Slider (sample size of deltamrr)
                    autoMark.GetnInputSld = Configure.GetInputSld; // X number to compare input slider                  
                   //Start Mark Bad Slider
                                 
                    autoMark.StartAutoMark(FileName , MonPath, PreRDDPath, EDBPath, RTTCPath, OCRPath , myConn , emailaddress);

                    if (autoMark.RetST == -7 || autoMark.RetST == -8)
                    {
                        fileInfo = GetFileInfo(MonPath); // ReGet file mark ini 
                        
                        if (fileInfo.Length > 1)
                        {
                            i = 0;
                            autoMark.Sleep(5000);
                            continue;
                        }
                        else
                        {
                            for (int x = 0; x < 10; i++)
                            {
                                autoMark.Sleep(720);  // Threading Sleep for waiting update RTTC Database 
                            }                           
                        }
                    }
                    else
                    { 
                        if (string.IsNullOrEmpty(autoMark.RetErr))  // Collect ini file 
                        {
                            //No error
                            IniFileCollector(completePath, FileFullName, FileName, 0);
                            File.Delete(FileFullName);
                        }
                        else     //If have an error will be writed the log file  & Collect MrkSld ini file event uncomplete mark slider  !!!
                        {
                            IniFileCollector(notCompletePath, FileFullName, FileName,1);
                            WRLog wrlog = new WRLog();
                            wrlog.WriteLogFile(Application.StartupPath, "Error Slider was marked.ini", autoMark.RetErr + "Err.Num=" + autoMark.RetST);
                            autoMark.Sleep(1000);
                        }
                        //update info                              
                        updateinfo((autoMark.setStartTime.ToString()), (autoMark.setEndTime.ToString()), autoMark.setProduct, autoMark.setTester, autoMark.RetErr, autoMark.RetNMarkedSld);
                        Application.DoEvents();
                    }

                }

                //Disconnect server
                bool closeDB =Configure.CloseDatabase;

                //Stamp Yarm Mon
                if (DateTime.Now > _yarmstamptime)
                {
                    Yarm yarm = new Yarm();
                    yarm.SetYarmPath = Configure.GetYarmServer;
                    yarm.SetYarmuser = Configure.GetYarmUser;
                    yarm.SetStampYarm = _exeName; //stamp yarm
                    _yarmstamptime = DateTime.Now.AddMinutes(5);
                }
               // Application.DoEvents();
            }
            catch (Exception ex)
            {
                updateinfo("-", "-", "-", "-", ex.Message, 0);
                RefershInfo();
                WRLog wrlog = new WRLog();
                wrlog.WriteLogFile(Application.StartupPath, "Error AutoMark is starting.ini", ex.Message);   
            }
            timerMonitor.Enabled = true;
        }
        private void IniFileCollector(string Path , string fullFileName, string fName , int status)
        {
            //status = 0 >> Complete 
            //status = 1 >> Not complete
            string[] rawData = null;
            CGetMemSetting getcontens = null;

            if (status == 0)
            {
                if (!Directory.Exists(Path)) { Directory.CreateDirectory(Configure.GetCompletedPath); }
            }
            else
            {
                if (!Directory.Exists(Path)) { Directory.CreateDirectory(Configure.GetNotCompletePath); }

            }            
                rawData = File.ReadAllLines(fullFileName);
                getcontens = new CGetMemSetting(rawData, fullFileName);
                File.WriteAllLines(Path + fName, rawData);

                System.Threading.Thread.Sleep(100);
                File.Delete(fullFileName);     
        }
        private void updateinfo(string strTime , string endTime, string product , string tester, string comments , int nMark)
        {        
            lbStrT.Text = strTime; 
            lbET.Text =  endTime; 
            lbProd.Text = product; 
            lbTst.Text = tester;
            if (strTime == "-" && nMark == 0)
            {
                lbnMark.Text = "-";
            }
            else
            {
                lbnMark.Text = nMark.ToString();
            }
         
            lbStrT.ForeColor = Color.Blue;
            lbET.ForeColor = Color.Blue;

            if (!string.IsNullOrEmpty(comments))
            {
                lbErr.Text = comments; 
                lbErr.ForeColor = Color.Red;
            }
            else { lbErr.Text = "No Error"; }
            lbStrT.Show();
            lbET.Show();
            lbProd.Show();
            lbTst.Show();
            lbnMark.Show();
            lbErr.Show();
        }
        private void RefershInfo()
        {
            lbStrT.Refresh();
            lbET.Refresh();
            lbProd.Refresh();
            lbTst.Refresh();
            lbnMark.Refresh();
         //   lbNlot.Refresh();
            lbErr.Refresh();

        }

        private FileInfo[] GetFileInfo(string MonPath)
        {
            DirectoryInfo drInfo = new DirectoryInfo(MonPath);
            FileInfo[] fInfo = drInfo.GetFiles("*.ini");
            return fInfo;          

        }
    }
}
