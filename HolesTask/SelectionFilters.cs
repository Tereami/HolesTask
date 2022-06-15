using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace HolesTask
{
    public class constrFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class mepFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Duct) return true;
            if (elem is Pipe) return true;
            if (elem is Autodesk.Revit.DB.Electrical.CableTray) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }


    public class taskFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is FamilyInstance)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
             return false;
        }
    }

}
