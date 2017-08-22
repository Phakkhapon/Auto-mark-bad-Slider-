using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lib_Net;
using MySql.Data.MySqlClient;
using System.Data;


namespace AutoMarkDCTFile
{
    class GetRawData
    {
        //Fields 
        #region Field 
        DataTable _retRawData , _tabheader, _tabparam;
        MySqlConnection _conn;
        string _productName;
        bool _includeCF;
        #endregion
        // Properties 
        #region Properties     
        public DataTable setHeader { set { _tabheader = value; } }
        public DataTable setParam { set { _tabparam = value; } }
        public MySqlConnection setConn { set { _conn = value; } }
        public string setProdName { set { _productName = value; } }
        public bool setIncludeCF { set { _includeCF = value; } }

        public DataTable ReturnRawData {
            get {
                //get raw data from RTTC database
                if (!string.IsNullOrEmpty(_conn.ConnectionString)) //connection string is not emtry 
                {
                    _retRawData = getData();
                }
                else {
                    _retRawData = null;
                }
                return _retRawData;
            }
        }      
        #endregion

        #region Methode 
        private DataTable getData()
        {
            DataTable temp = null;
            string sql = "SELECT";
            sql = sql + "'" + _productName + "' ProductName,";
            sql = queryHeader(sql);   // query header 

            if (!string.IsNullOrEmpty(sql))
            {
                sql = queryParam(sql);
            }
            return temp;
        }
  
        private string queryHeader(string sql)
        {
            string sqlHeader = sql;
            if (_tabheader.Rows.Count > 0)
            {
                int maxheader = _tabheader.Rows.Count; //set maximum number of header
                for (int i = 0; i < maxheader; i++)
                {
                    string header = _tabheader.Rows[i]["header"].ToString();
                    if (header == "BARNO")
                    {
                        sqlHeader = sqlHeader + "Mid(a.HGA_SN,5,1) SideNo,";
                        sqlHeader = sqlHeader + "Mid(HGA_SN,6,2) BarNo,";
                        sqlHeader = sqlHeader + "Right(HGA_SN,1) LocNo,";
                    }
                    else { sqlHeader = sqlHeader + "a." + header + ","; }
                }
            }
            else { sqlHeader = null; }
            return sqlHeader;
        }

        private string queryParam(string sql)
        {
            string sqlParam = sql;
            if (_tabparam.Rows.Count > 0)
            {
                int maxpara = _tabparam.Rows.Count;            
                for (int i = 0; i < maxpara; i++)
                {
                    string paraname = _tabparam.Rows[i]["param_rttc"].ToString();
                    string displayname = _tabparam.Rows[i]["param_display"].ToString();
                    string machineCF = null;
                    if (string.IsNullOrEmpty(_tabparam.Rows[i]["MachineCF"].ToString()))
                    {
                        machineCF = null;
                    }
                    else { machineCF = _tabparam.Rows[i]["MachineCF"].ToString(); }
                    sqlParam = sqlParam + "B." + paraname + " As " + displayname + ",";
                    //Need CF ? 
                    if (_includeCF==true)
                    {
                       
                        bool cfAdd = bool.Parse(_tabparam.Rows[i]["param_add"].ToString());
                        bool cfMul = bool.Parse(_tabparam.Rows[i]["param_mul"].ToString());
                        if (cfAdd || !string.IsNullOrEmpty(machineCF))
                        {
                            sqlParam = sqlParam + "C." + paraname + "" + displayname + ".CFAdd,";
                        }
                        if (cfMul || !string.IsNullOrEmpty(machineCF))
                        {
                            sqlParam = sqlParam + "D." + paraname + "" + displayname + ".CFMul,";
                        }
                        if (cfAdd || cfMul )
                        {
                            sqlParam = sqlParam + "(SELECT " + paraname + " FROM db_" + _productName + ".tabfactor_media WHERE Media_PN = LEFT(A.MediaSN, 3) AND Media_Group = SUBSTRING(A.MediaSN, 4, 1) AND Media_Serial = SUBSTRING(A.MediaSN, 5, 3)) " + displayname + ".CFMedia" + "," ;
                        }
                    }
                }
            }
            return sqlParam;
        }
        #endregion

    
    }
}
