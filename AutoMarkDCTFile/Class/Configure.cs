using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;

namespace AutoMarkDCTFile
{
  public static class Configure
    {
        static string _iniPath, _server, _user,_pw,_port,_yarmServer , _yarmUser; //_owner,
        static string _monitorpath, _edbPath , _rttcPath, _OCRPath , _errPath ,_completedPath, _markLotPath;
        static string _database ,_PreRddPath , _notcompletePath;
        static string _nSlider;
        static MySqlConnection _MySqlRTTCDataConn;
        #region Properties
        public static string SetIniFile { set { _iniPath = value; } }
        public static string GetServer { get { return _server; } }
        public static string GetUser { get { return _user; } }
        public static string GetPassword { get { return _pw; } }
        public static string GetPort { get { return _port; } }
       // public static string GetOwner { get { return _owner; } }
        public static string GetYarmServer { get { return _yarmServer; } }
        public static string GetYarmUser { get { return _yarmUser; } }
        public static string GetMonitorPath { get { return _monitorpath; } }
        public static string GetPreRDDPath { get { return _PreRddPath; } }  //Store dct files
        public static string GetEDBPath { get { return _edbPath; } }  //Store dct files
        public static string GetRTTCPath { get { return _rttcPath; } }  //Store dct files

        public static string GetOCRPath { get { return _OCRPath; } }  //write Serial failure 

        public static string GetErrPath { get { return _errPath; } }  
        public static string GetCompletedPath { get { return _completedPath; } }  
        public static string GetMarkLotPath { get { return _markLotPath; } }
        public static string GetNotCompletePath { get { return _notcompletePath; } }
        public static string GetNSlider { get { return _nSlider; } }

        //SQL Management 
        public static MySqlConnection GetRTTCStringConnection
        {
            get
            {   
                if (!string.IsNullOrEmpty(_MySqlRTTCDataConn.ConnectionString))
                {
                    return _MySqlRTTCDataConn;
                }
                else
                {
                    SetRTTCStringConnecttion();
                    return _MySqlRTTCDataConn; }                 
            }
        }
        public static Boolean OpenDatabase
        {         
            get
            {
                Boolean ChkConnection = false;
                ChkConnection = OpendDatabaseConnection();
                return ChkConnection;
            }
        }
        public static Boolean CloseDatabase
        {
            get
            {
                Boolean ChkConnection = false;
                ChkConnection = CloseDatabaseConnection();
                return ChkConnection;
            }
        }
        public static Boolean ReOpenDatabase
        {
            get
            {
                Boolean ChkConnection = false;
                ChkConnection = ReOpenDatabaseConnection();
                return ChkConnection;
            }
        }

        //----------end SQL Management------------
        public static string LoadConfigure()
        {
            string strError;
            if (!string.IsNullOrEmpty(_iniPath))
            {
                LoadFileConfigure(_iniPath);
                strError = null;
            }
            else
            {
                strError = "Cannot Acess Configuration File";
            }
            return strError;
        }
        public static string SetDatabase
         {
            set { _database = value; }
         }     
         public static string GetConnString
        {
            get {
                string connstring =null;
                if (string.IsNullOrEmpty(_server))
                {
                    return "Server is not correct";
                }
                else
                {
                    connstring= SetConnString();
                    return connstring;
                }
            }           
        }


        private static string _emailaddr;
        public static string GetEmailAddress
        {
            get { return _emailaddr; }
            set { _emailaddr = value; }
        }


        private static string _nInputSld;
        public static string GetInputSld
        {
            get { return _nInputSld; }
            set { _nInputSld = value; }
        }

        #endregion

        #region Methode
        private static void LoadFileConfigure(string iniPath)
        {
            if (File.Exists(iniPath))
            {
                IniFile ini = new IniFile(iniPath);
                _server = ini.Read("Server", "RTTC_Database");
                _user = ini.Read("User", "RTTC_Database");
                _pw = ini.Read("Password", "RTTC_Database");
                _port = ini.Read("Port", "RTTC_Database");
               // _owner = ini.Read("Owner", "Yarm");
                _yarmServer = ini.Read("ServerYarm", "Yarm");
               _yarmUser = ini.Read("Owner", "Yarm");

                _monitorpath = ini.Read("MonitorPath", "Folder");
                _PreRddPath = ini.Read("PreRDDPath", "Folder");
                _edbPath = ini.Read("EDBPath", "Folder");
                _rttcPath = ini.Read("RTTCPath", "Folder");
                _OCRPath = ini.Read("OCRPath", "Folder");

                _errPath = ini.Read("Error", "ResultCollectionInfo");
                _completedPath = ini.Read("Done", "ResultCollectionInfo");
                _markLotPath = ini.Read("MarkedLot", "ResultCollectionInfo");
                _notcompletePath = ini.Read("IniNotComplete", "ResultCollectionInfo");

                _emailaddr = ini.Read("emailaddr", "EmailAddress");
                _nSlider = ini.Read("nsld", "SliderQty");
                _nInputSld = ini.Read("nInputSld", "SliderQty");


            }
            else
            {
                InitialConfiure(_iniPath);
            }
        }
        private static void InitialConfiure(string IniPath)
        {
            IniFile ini = new IniFile(IniPath);
            ini.Write("Server", "wdtbtsd13", "RTTC_Database");
            ini.Write("User","rttc_net", "RTTC_Database");
            ini.Write("Password","rttc_net" ,"RTTC_Database");
            ini.Write("Port","3306","RTTC_Database");

            ini.Write("MonitorPath", "R:\\MarkReject\\Database\\","Folder");
            ini.Write("PreRDDPath", "R:\\AutoMark2RDD\\","Folder");
            ini.Write("EDBPath", "\\\\wdtbhgafs01\\MRdata\\Prime\\", "Folder");
            ini.Write("RTTCPath", "\\\\wdtbhgafs01\\WEXfile\\Prime\\", "Folder");

            ini.Write("OCRPath", "\\\\wdtbtsd12\\PE_Sn\\Fail_14\\", "Folder");          
            ini.Write("Error", "D:\\AutoMarkLOG\\ERR\\","ResultCollectionInfo");
            ini.Write("Done", "D:\\AutoMarkLOG\\Complete\\","ResultCollectionInfo");
            ini.Write("MarkedLot", "D:\\AutoMarkLOG\\IniFile\\", "ResultCollectionInfo");
            ini.Write("IniNotComplete", "D:\\AutoMarkLOG\\IniFile\\", "ResultCollectionInfo");

            //stamp yarm 
            ini.Write("Owner", "Phakkhapon_w", "Yarm");
            ini.Write("ServerYarm", "\\\\wdtbvmmag59\\TE_SharedFiles\\TimeStamp\\ServerApp\\", "Yarm");

            ini.Write("nsld", "75", "SliderQty");
            ini.Write("nInputSld", "25", "SliderQty");
        }
        
        #region connectionstring 
        private static string SetConnString()
        {
            string sqlconn = "server=" + _server + ";";
            sqlconn = sqlconn + "uid=" + _user + ";";
            sqlconn = sqlconn + "pwd=" + _pw + ";";
            if (string.IsNullOrEmpty(_database))
            {
                sqlconn = sqlconn + "port=" + _port + ";";
                sqlconn = sqlconn + "Connection Lifetime=15" + ";";
            }
            else
            {
                sqlconn = sqlconn + "database=" + _database + ";";
                sqlconn = sqlconn + "port=" + _port + ";";
                sqlconn = sqlconn + "Connection Lifetime=15" + ";";
            }
            return sqlconn;
        }
        private static string SetRTTCStringConnecttion()
        {
            string Err = null;
            try
            {
                if (_MySqlRTTCDataConn == null)
                {
                    _MySqlRTTCDataConn = new MySqlConnection();
                }
                if (_MySqlRTTCDataConn.State == ConnectionState.Open) { _MySqlRTTCDataConn.Close(); }
                _MySqlRTTCDataConn.ConnectionString = SetConnString();
                Err = null;
            }
            catch (Exception ex)
            {
                Err = ex.Message;
            }
            return Err;
        }
        private static Boolean OpendDatabaseConnection()
        {
            Boolean chkState = false;
            if (_MySqlRTTCDataConn == null)
            {
                _MySqlRTTCDataConn = new MySqlConnection();
            }

            if (_MySqlRTTCDataConn.State == ConnectionState.Closed)
            {
                SetRTTCStringConnecttion();
                _MySqlRTTCDataConn.Open();
            }
            if (_MySqlRTTCDataConn.State == ConnectionState.Open)
            {
                chkState = true;
            }
            else { chkState = false; }
            return chkState; 
        }
        private static Boolean CloseDatabaseConnection()
        {
            Boolean chkState = false;
            if (_MySqlRTTCDataConn == null)
            {
                _MySqlRTTCDataConn = new MySqlConnection();
            }
            if (_MySqlRTTCDataConn.State == ConnectionState.Open)
            {
                SetRTTCStringConnecttion();
                _MySqlRTTCDataConn.Close();
            }
            if (_MySqlRTTCDataConn.State == ConnectionState.Closed)
            {
                chkState = true;
            }
            else { chkState = false; }
            return chkState;
        }
        private static Boolean ReOpenDatabaseConnection()
        {
            Boolean chkState = false;
            chkState = CloseDatabaseConnection();
            chkState = OpendDatabaseConnection();
            return chkState;
        }
        #endregion

    #endregion
    }

}
