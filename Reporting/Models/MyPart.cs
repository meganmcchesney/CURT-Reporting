using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Reporting.Models
{
    public class MyPart
    {
        public int PartID { get; set; }
        public string InstallSheet { get; set; }
        public string path { get; set; }
        public string dateModified { get; set; }
        public List<string> Images { get; set; }
        public List<string> Videos { get; set; }
    }
}