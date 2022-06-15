using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HolesTask
{
    public class XMLinfo
    {
        public string MepFileName;
        public string ConstrFileName;
        public HostInfo HInfo;
        public LocationInfo LocInfo;

        public FamilyInstance task;

        public XMLinfo(string mepFileName, string constFileName, HostInfo hostInfo, LocationInfo locInfo)
        {
            MepFileName = mepFileName;
            ConstrFileName = constFileName;
            HInfo = hostInfo;
            LocInfo = locInfo;
        }
    }
}
