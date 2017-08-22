using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace AutoMarkDCTFile
{
    class Yarm
    {
        #region Filed 
        string _yarmpath;  //= Configure.GetYarmServer;
        string _yarmstampuser;  //= Configure.GetYarmUser;
        string _fileExeName;
        #endregion

        #region Properties
        public string SetYarmPath { set { _yarmpath = value; } }
        public string SetYarmuser { set { _yarmstampuser = value; }  }
        public string SetStampYarm
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _fileExeName = value;
                    StampYarm();
                }
            }
        }

        #endregion

        #region Methode 
        protected Boolean StampYarm()
        {
            Boolean chkStamp = false;
            try
            {
                string Detector;
                string ComputerName = System.Environment.MachineName;
                string StampPath = _yarmpath;
                StampPath = StampPath + _fileExeName + ".ckt";
                Detector = Convert.ToString(DateTime.Now) + ",300," + ComputerName + "," + _yarmstampuser;
                IniFile ini = new IniFile(StampPath);
                ini.Write(_fileExeName, Detector, "Crunching");
                chkStamp = true;
            }
            catch (Exception)
            {
                chkStamp = false;
            }
            return chkStamp;
        }

        #endregion
    }
}
