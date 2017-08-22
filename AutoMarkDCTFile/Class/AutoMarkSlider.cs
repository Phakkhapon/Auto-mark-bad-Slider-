using System;
using System.Data;
using System.IO;
using Lib_Net;
using MySql.Data.MySqlClient;
using MyLibByCCharp;

namespace AutoMarkDCTFile
{
    class AutoMarkSlider
    {
        struct sMakeDetials
        {
            public string product;
            public string tester;
            public DateTime starttime;
            public DateTime endtime;
            public string markGrdID;
            public string markGrdName;
        }

        #region Fields 
        private string _err = null;
        private int _ret = 0; // Default is "0"
        private int _nMarkedSld = 0;
        private string _dctFolder=null;
        private string _nsld;
        private DataTable _StampstatusTable = new DataTable();
        #endregion

        #region Properties
        sMakeDetials smk;
        public string setProduct{ get { {return smk.product; } } set {  smk.product = value;} }
        public string setTester { get { { return smk.tester; } } set { smk.tester = value; } }
        public DateTime setStartTime { get { return smk.starttime; } set { smk.starttime = value;}}
        public DateTime setEndTime { get { { return smk.endtime; } } set { smk.endtime = value; } }
        public string setMarkGrdID { set { smk.markGrdID = value; } }
        public string setMarkGrdName { set { smk.markGrdName = value; } }
        public string RetErr { get { return _err; } }  // Return ERR 
        public int RetST { get { return _ret; } }  // Return number err trakcing
        public int RetNMarkedSld { get { return _nMarkedSld; } }  // Return n Marked slider 
        public string SetNSldSetting { set { _nsld = value; } }
        

        private string _appPath;
        public string SetAppPath
        {           
            set { _appPath = value; }
        }

        private string _nInputSld;
        public string GetnInputSld
        {
            set { _nInputSld = value; }
        }

        #endregion

        #region Methode 
        public void StartAutoMark(string fIniName , string MonPath, string PreRDDPath, string EDBPath, string RTTCPath, string OCRPath ,MySqlConnection mySqlConn , string emailaddress)
        {
            try
            {

                if (mySqlConn.State == ConnectionState.Closed) { mySqlConn.Open(); } //if connection is closed then reOpen connection 
                CParameterRTTCMapping RTTCParam = new CParameterRTTCMapping(mySqlConn);
                DataTable HeaderDetials = RTTCParam.GetHeaderDetail();
                DataTable Header = HeaderDetials.DefaultView.ToTable(false, "HeaderName");
                Header.Columns["HeaderName"].ColumnName = "Header";

                DataTable Param = RTTCParam.GetParamByProduct(smk.product, true, CParameterRTTCMapping.enuOrderParameterBy.eByOrderID,false);
                DataTable RawData = getRawdata(Header, Param,mySqlConn);

                int chkWrDCT = 0;
                int nmarked = 0;
                //---------Email info-------
                string[] getemail = emailaddress.Split(',');
                string sendfrom = "AutoMarkSlider@wdc.com";
                string mailsubject = null;
                string contents = null;
                //--------------------------

                if (RawData.Rows.Count > 0)  //Raw Data 
                {
                    int nrow = RawData.Rows.Count;          
                    if ( nrow > int.Parse(_nInputSld))
                    {
                        if (nrow < int.Parse(_nsld) && nrow > int.Parse(_nInputSld))
                        {
                            WRLog writelog = new WRLog();
                            writelog.WriteLogFile(_appPath, "Check Raw Data from Database.ini", "Slider quantiry is less than 75 slider: "
                                + smk.product + "_" + smk.starttime + "_" + smk.endtime + "_" + smk.tester);

                            mailsubject = "Check Input: Slider to mark deltaMRR" + " Product: " + smk.product + " Tester:MT" + smk.tester;
                            contents = "Input Slider (" + RawData.Rows.Count + ") is less than " + _nsld + " slider;" + "Product " + smk.product + ";" + "Tester:MT" + smk.tester + ";"
                             + "Start_Time:" + smk.starttime + ";" + "End_Time:" + smk.endtime + ";";
                            bool retsendemail = SendEmail(getemail, emailaddress, sendfrom, mailsubject, contents);
                            if (retsendemail == false)
                            { writelog.WriteLogFile(_appPath, "SendingEmailError.ini", _err + "_" + contents); }
                        }

                        _StampstatusTable = InitialCheckingWRDCTTable(RawData);
                        _StampstatusTable.TableName = smk.product;
                        CGetMemSetting[] getDCTf = GetDCTData(smk.product, RawData, HeaderDetials, Param);
                        if (_ret == -3)
                        {
                            chkWrDCT = _ret;   // need collect any error from class CConvertDB2DCT
                            mailsubject = "Check Class Convert DB2DCT File";
                            contents = "CConvertDB2DCT was error : " + _err;
                            bool retsendemail = SendEmail(getemail, "Phakkhapon.wonganu@wdc.com", sendfrom, mailsubject, contents);
                        }
                        else
                        {
                            //_nMarkedSld = getDCTf.Length;
                            for (int i = 0; i < getDCTf.Length; i++)
                            {
                                chkWrDCT = WriteDCTFile(PreRDDPath, getDCTf[i], ref nmarked);
                                if (chkWrDCT == -2)// ReWrite dct file 
                                {
                                    Sleep(2000);
                                    chkWrDCT = WriteDCTFile(PreRDDPath, getDCTf[i], ref nmarked);
                                }
                            }
                        }

                        int cntUnmark = 0;  // n time to mark dct file            
                        DataTable remarksld = CheckWroteDCTFileResult(_StampstatusTable, ref cntUnmark);
                        if (remarksld.Rows.Count > 0 && chkWrDCT == 0) // Check result of write dc file
                        {

                            getDCTf = GetDCTData(smk.product, remarksld, HeaderDetials, Param);
                            for (int i = 0; i < getDCTf.Length; i++)
                            {
                                chkWrDCT = WriteDCTFile(PreRDDPath, getDCTf[i], ref nmarked);
                                if (chkWrDCT == -2)// ReWrite dct file 
                                {
                                    Sleep(2000);
                                    chkWrDCT = WriteDCTFile(PreRDDPath, getDCTf[i], ref nmarked);
                                }
                            }
                            //recheck rewrite dct file
                            remarksld = CheckWroteDCTFileResult(_StampstatusTable, ref cntUnmark);
                            if (cntUnmark > 0) //if n time more than > 1 then alert mail to MTCI to verify the system
                            {
                                WRLog writelog = new WRLog();
                                writelog.WriteLogFile(_appPath, "Remark Slider.ini", "Remark slider more than 1 time: "
                                    + smk.product + "_" + smk.starttime + "_" + smk.endtime + "_" + smk.tester);

                                mailsubject = "Remark deltaMRR " + " Product: " + smk.product + " Tester:" + smk.tester;
                                contents = "Remark Delta MRR more thane " + cntUnmark + "time;" + "Product: " + smk.product + ";" + "Tester:MT" + smk.tester + ";"
                                     + "Start_Time:" + smk.starttime + ";" + "End_Time:" + smk.endtime + ";";
                                bool retsendemail = SendEmail(getemail, emailaddress, sendfrom, mailsubject, contents, remarksld);
                                if (retsendemail == false)
                                { writelog.WriteLogFile(_appPath, "SendingEmailError.ini", _err + "_" + contents); }
                            }
                        }
                        _nMarkedSld = CheckSliderMarked(_StampstatusTable);
                        if (_nMarkedSld < RawData.Rows.Count)  //n marked slider (dct ) < n input slider
                        {
                            mailsubject = "Check Quntity marked slider" + " Product: " + smk.product + " Tester:" + smk.tester;
                            contents = "Quantity marked slider is less than Input data ;" + "Product: " + smk.product + ";" + "Tester:MT" + smk.tester + ";"
                                + "Start_Time:" + smk.starttime + ";" + "End_Time:" + smk.endtime + ";" + "Mark Complete:" + _nMarkedSld + " slider from " + RawData.Rows.Count + " slider;";
                            bool ret = SendEmail(getemail, emailaddress, sendfrom, mailsubject, contents, remarksld);
                        }

                        if (chkWrDCT == 0) { _ret = Update2RDD(EDBPath, RTTCPath, _dctFolder); }
                        else { _ret = chkWrDCT; }
                        //write SN on wdtbtsd12
                        _ret = WRMarkedSN(RawData, OCRPath, fIniName);
                    }
                    else
                    {
                        _ret = -8; // Q'ty Slider of raw data (from RTTC Database) is less than 1/3 of N Input Slider Setting 
                    }
                }
                else
                {
                    _ret = -7;   // no data from RTTC Database 

                }
            }
            catch (Exception ex)
            {
               _err = ex.Message;

            }
        }
        private DataTable getRawdata(DataTable Header, DataTable Param, MySqlConnection mySqlConn)
        {
            DataTable rawdata = null;
            DataTable tabTester = new DataTable("Tester");
            tabTester.Columns.Add("Tester");
            tabTester.Rows.Add(smk.tester);
            // Call Get Raw data
            CGetRawData getdata = new CGetRawData(mySqlConn);
            rawdata = getdata.GetRawDataByParamByTester(smk.product, Header, Param,tabTester,null,smk.starttime, smk.endtime,false,null);
            return rawdata;
        }
        private CGetMemSetting[] GetDCTData(string prodname , DataTable RawData , DataTable HeaderDetials, DataTable Param)
        {
            CGetMemSetting[] getmem =null;
            CConvertDB2DCT converter;
            try
            {
               converter = new CConvertDB2DCT(prodname, RawData, HeaderDetials, Param);
               getmem = converter.GetAllConversionFile();               
            }
            catch (Exception ex)
            {
                _err = ex.Message;
                _ret = -3;
            }
            return getmem;
        }
        //write dct file for deltaMRR 
        private int WriteDCTFile(string PreRDDPath, CGetMemSetting DCTFiles , ref int nMarked)
        {
            int Ret = 0;
            string SldSN = DCTFiles.GetValueString("Header", "HeadSN");
            try
            {
                DCTFiles.DeleteSection("Avg_2");
                DCTFiles.DeleteSection("Nrm_2");
                DateTime strTime = DateTime.Parse(DCTFiles.GetValueString("Header", "StartTime"));
                strTime = strTime.AddSeconds(1); 
                string shoeNum = DCTFiles.GetValueString("Header", "ShoeNum");  
                DCTFiles.WriteValueString("Header", "StartTime", strTime.ToString("yyyy-MM-dd HH:mm:ss"));
                DCTFiles.WriteValueString("Header", "GradeName", smk.markGrdName);
                DCTFiles.WriteValueString("Header", "Remark", "RTTCAutoMarkReject");

                string TstNum = DCTFiles.GetValueString("Header", "Station", "0000");
                string fName = (strTime.ToString("yyyyMMddHHmmss")) + TstNum + shoeNum  + ".dct";
                _dctFolder = PreRDDPath + "B" + TstNum + @"\";

                // Write DCT Files to preRDD folder for sending to EDB/RTTC
                if (!Directory.Exists(_dctFolder)) { Directory.CreateDirectory(_dctFolder); }
                DCTFiles.WriteStringToFile(_dctFolder + fName); 
                Ret = 0;
                nMarked = nMarked + 1 ;

                //Stamp Status of WR DCT file  1 = success               
                StampStatusWrDCTFile( SldSN, 1, "StampMarkSld");  
            }
            catch (Exception ex)
            {
                //If unsuccessful to write dct file to be stamped 0 = not success
                StampStatusWrDCTFile( SldSN, 0 , "StampMarkSld");
                Ret = -2;  // Cannot write dct file.
                _err = "Ret.Num=" + Ret + " Err.msg=" + ex.Message + "_Write DCT File";
            }
            return Ret;
        }
        private int Update2RDD(string EDBPath, string RTTCPath,string PreRDD)
        {
            int Ret = 0;         
            try
            {               
                DirectoryInfo pathDir = new DirectoryInfo(PreRDD);
                // re-update to RDD 
                reupload:
                FileInfo[] targetFile = pathDir.GetFiles("*.dct");
                string[] rawDCT = null;
                CGetMemSetting dctfiles = null;
                string TstNum = null;
                    for (int i = 0; i < targetFile.Length; i++)
                    {
                    int chkWr = 0;
                    string fileName = targetFile[i].ToString();
                        string fullName = targetFile[i].FullName.ToString();
                        if (Directory.Exists(_dctFolder))
                        {
                            rawDCT = File.ReadAllLines(fullName);
                            dctfiles = new CGetMemSetting(rawDCT, fullName);
                            TstNum = dctfiles.GetValueString("Header", "Station");
                            string CopyToEDB = EDBPath + "B" + TstNum + @"\";
                            string CopyToRTTC = RTTCPath + "B" + TstNum + @"\";

                             Sleep(10);
                            try
                            {
                                if (!File.Exists(CopyToEDB)) { Directory.CreateDirectory(CopyToEDB); }
                                if (!File.Exists(CopyToRTTC)) { Directory.CreateDirectory(CopyToRTTC); }
                               //Update EDB
                                File.WriteAllLines(CopyToEDB + fileName, rawDCT);                            
                                Sleep(10);
                                //Update RTTC
                                File.WriteAllLines(CopyToRTTC + fileName, rawDCT);                             
                            }
                            catch (Exception)
                            {                            
                                chkWr = -9;
                                continue;  // next file if found an error                                
                            }
                        }
                        if (chkWr == 0) { File.Delete(fullName); }  // Update RDD complete will be removed 

                    }
                    // Re-update RDD                    
                    if (targetFile.Length > 0)
                    {
                        Sleep(100);
                        goto reupload;  // upload the remaining files 
                    }
            }
            catch (Exception ex)
            {
                _ret = -2;
                _err = "Ret.Num=" + _ret + " Err.msg=" + ex.Message + "_StartMarkSlider";
            }
            return Ret;
        }
        //Sleep Time 
        public void Sleep(int timesleep)
        {
            System.Threading.Thread.Sleep(timesleep);        
        }
        //Write all Serials to \\wdtbtsd12\PE_Sn\Fail_14 before send to EDB&RTTC Database.
        private int WRMarkedSN(DataTable rawData , string OCRPath, string fName)
        {
            int RetWrSn = 0;
            string ocrName = fName; 
            try
            {
                if (rawData.Rows.Count > 0)
                {
                    ManageString mngstring = new ManageString();
                    string chkpath = mngstring.Right(OCRPath);
                    if (string.Compare(chkpath, @"\") != 0) { OCRPath = OCRPath + @"\"; }
                    if (!Directory.Exists(OCRPath)) { Directory.CreateDirectory(OCRPath); }

                    if (!string.IsNullOrEmpty(ocrName) && ocrName.Contains(".ini"))
                    {
                        ocrName = ocrName.Replace(".ini", "");     
                    }
                    else
                    {   // set default name                
                        ocrName = "OCRNAME_" + (DateTime.Now).ToString("yyyyMMddHHmmss");
                    }
                    //write Serial GRD 14 for OCR 
                    CGetMemSetting getmem = new CGetMemSetting(OCRPath + ocrName);
                    for (int i = 0; i < rawData.Rows.Count; i++)
                    {
                        string sn = rawData.Rows[i]["Hga_SN"].ToString();
                        string newGrd = "14";
                        getmem.WriteValueString("SERIAL", sn, newGrd);                                       
                    }                             
                    getmem.WriteStringToFile(OCRPath + ocrName + ".txt");
                    //Recheck Result
                    RetWrSn = CheckOCRFile(OCRPath, ocrName + ".txt");
                }
                else
                {
                    RetWrSn = -4;
                    _err = "No Data is to be written OCR SN";
                }
            }
            catch (Exception ex)
            {
                RetWrSn = -5;
                _err = "RetErr.Num= " + RetWrSn + "err.msg=" + ex.Message + "_WRMarkedSN";
                throw;
            }
            return RetWrSn;
        }
        private int CheckOCRFile(string OCRPath , string OCRFileName)
        {
            int RetNum = 0;
            // recheck ocr file 
            DirectoryInfo dir = new DirectoryInfo(OCRPath);
            FileInfo[] chkfile = dir.GetFiles(OCRFileName);
            if (chkfile.Length > 0)
            {
                RetNum = 0;  // Success 
            }
            else
            {
                RetNum = -6;  // Not Success 
                _err = "Write OCR Serial not complete" + "_WRMarkedSN";
            }
            return RetNum;
        }

        private DataTable InitialCheckingWRDCTTable(DataTable rawdata)
        {
            DataTable dt = new DataTable();
          // dt = rawdata.Copy();
            DataView dv = new DataView(rawdata);
            dt = dv.ToTable("dt", false,"ProductName","Test_time", "Tester", "Spec", "Lot", "GradeName", "Hga_SN");
            dt.Columns.Add("StampMarkSld", typeof(int));
            return dt;
        }
      
        private int StampStatusWrDCTFile(string SldSN , int Status, string stampCol)
        {
            int ret=0;
            try
            {
                if (_StampstatusTable.Rows.Count > 0)
                {
                    for (int i = 0; i < _StampstatusTable.Rows.Count; i++)
                    {
                        string sn = _StampstatusTable.Rows[i]["Hga_sn"].ToString();               
                        if (string.Compare(sn, SldSN) == 0 && _StampstatusTable.Rows[i]["StampMarkSld"] == DBNull.Value )
                        {
                            _StampstatusTable.Rows[i][stampCol] = Status;
                            break;
                        }
                    }                    
                } 
            }
            catch (Exception ex)
            {
                ret= -1;
                _err = "StampStatusWRDCTFile : " + ex.Message;
            }
            return ret;
        }

        private DataTable CheckWroteDCTFileResult(DataTable resultdata , ref int cntUnmark)
        {      
            DataTable temp =null;
            if (resultdata.Rows.Count > 0)
            {
                resultdata.DefaultView.RowFilter = "StampMarkSld = 0";
                temp = resultdata.DefaultView.ToTable();
                if (temp.Rows.Count > 0) { cntUnmark = cntUnmark +1; }  //Count if found unsuccess slider
            }
            return temp;
        }

        private int CheckSliderMarked(DataTable finalData)
        {
            int ret = 0;
            DataTable temp = null;
            if (finalData.Rows.Count > 0)
            {
                finalData.DefaultView.RowFilter = "StampMarkSld = 1";
                temp = finalData.DefaultView.ToTable();
                ret = temp.Rows.Count;
            }            
            return ret;
        }



        private bool SendEmail(string[] emaillist,string emailaddress,string sendfrom , string mailsubject ,string contents,DataTable rawdata = null)
        {
            bool send = false;
            SendingEmail sendmail = new SendingEmail();
            string retErr = null;
            int retmail;
            try
            {

                if (rawdata != null)
                {
                    retmail = sendmail.SendEmailThroughSMTP("develop_te", "0nef0rA110nef0rA1116",
                        emailaddress, sendfrom, mailsubject, contents , null, rawdata, true);
                    if (retmail < 0)
                    {
                        retErr = sendmail.RetError;
                        send = false;
                    }
                    else
                    {
                        send = true;
                    }
                }
                else
                {
                    retmail = sendmail.SendEmailThroughSMTP("develop_te", "0nef0rA110nef0rA1116",
                   emailaddress, sendfrom, mailsubject, contents, null, null, true);
                    if (retmail < 0)
                    {
                        retErr = sendmail.RetError;
                        send = false;
                    }
                    else
                    {
                        send = true;
                    }
                }
              
            }
            catch (Exception ex)
            {
                retmail = sendmail.SendEmailThroughSMTP("develop_te", "0nef0rA110nef0rA1116", "Phakkhapon.wonganu@wdc.com", sendfrom, 
                    "Sending Email was failed;", ex.Message  , null, null, true);
                _err = ex.Message;
               
            } 
      
            return send;

        }

        #endregion
    }

}
