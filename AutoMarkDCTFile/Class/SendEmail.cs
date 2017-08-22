using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarkDCTFile.Class
{
    public class SendEmail
    {

        private string _emailaddr;
        public string SetEmailAddress
        {
            get { return _emailaddr; }
            set { _emailaddr = value; }
        }

        private string _emailHost;
        public  string SetEmailHost
        {
            get { return _emailHost; }
            set { _emailHost = value; }
        }




    }
}
