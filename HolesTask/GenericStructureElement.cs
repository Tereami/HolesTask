using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;


namespace HolesTask
{
    public class GenericStructureElement
    {
        private bool _correct;
        public bool Correct
        {
            get { return _correct; }
        }

        private Element _elem;
        public Element Elem
        {
            get { return _elem; }
        }

        private double _thickness;
        public double Thickness
        {
            get { return _thickness; }
        }

        private Level _baseLevel;
        public Level BaseLevel
        {
            get { return _baseLevel; }
        }

        public GenericStructureElement(Element elem)
        {
            _elem = elem;
            _correct = false;

            if (elem is Wall)
            {
                Wall w = elem as Wall;
                _thickness = w.Width;

                Document doc = w.Document;
                _baseLevel = doc.GetElement(w.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsElementId()) as Level;
                _correct = true;
                return;
            }

            if(elem is Floor)
            {
                Floor f = elem as Floor;
                
                _thickness = f.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM).AsDouble();

                Document doc = f.Document;
                _baseLevel = doc.GetElement(f.get_Parameter(BuiltInParameter.LEVEL_PARAM).AsElementId()) as Level;

                _correct = true;
                return;
            }
        }
    }
}

