using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;

namespace AutoMarkDCTFile
{
class WRLog
    {
         public string WriteLogFile(string PathLog, string LogFileName, string TextForWriting)
        {
            string result = null;
            string fullName;
            DirectoryInfo DirCopyTo = new DirectoryInfo(PathLog);
            if (DirCopyTo.Exists == false) { Directory.CreateDirectory(PathLog); }
            if (PathLog.Substring(0, 1) != "\\") { PathLog = PathLog + "\\"; }
            fullName = PathLog + LogFileName;
            try
            {
                DateTime writeTime = DateTime.Now;
                File.AppendAllText(fullName, writeTime.ToString() + " " + TextForWriting + Environment.NewLine, UTF8Encoding.UTF8);
                result = "Complete";
            }
            catch (Exception ex) { result = ex.Message; }
            return result;
        }
    }
}
