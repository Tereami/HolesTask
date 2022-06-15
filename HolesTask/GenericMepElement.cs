using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;

namespace HolesTask
{
    public class GenericMepElement
    {
        private Element _elem;
        public Element elem
        {
            get { return _elem; }
            set { elem = value; }
        }

        private double _heigth;
        public double Heigth
        {
            get { return _heigth; }
        }

        private double _width;
        public double width
        {
            get { return _width; }
        }

        private Line _locationCurve;
        public Line locationCurve
        {
            get { return _locationCurve; }
        }

        private double _offset;
        public double offset
        {
           get { return _offset; }
        }

        private Level _baseLevel;
        public Level baseLevel
        {
            get { return _baseLevel; }
        }

        public GenericMepElement(Element elem)
        {
            _elem = elem;

            if(elem is Duct)
            {
                Duct d = elem as Duct;
                try
                {
                    _width = d.Width;
                    _heigth = d.Height;
                }
                catch
                {
                    _width = d.Diameter;
                    _heigth = d.Diameter;
                }

                LocationCurve lc = d.Location as LocationCurve;
                _locationCurve = lc.Curve as Line;

                _offset = d.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).AsDouble();
                Document doc = d.Document;
                _baseLevel = doc.GetElement(d.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId()) as Level;

                return;
            }

            if(elem is Pipe)
            {
                Pipe p = elem as Pipe;
                _width = p.Diameter;
                _heigth = p.Diameter;

                LocationCurve lc = p.Location as LocationCurve;
                _locationCurve = lc.Curve as Line;

                _offset = p.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).AsDouble();
                Document doc = p.Document;
                _baseLevel = doc.GetElement(p.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId()) as Level;

                return;

            }

            if (elem is CableTray)
            {
                CableTray ct = elem as CableTray;
                _width = ct.Width;
                _heigth = ct.Height;

                LocationCurve lc = ct.Location as LocationCurve;
                _locationCurve = lc.Curve as Line;

                _offset = ct.get_Parameter(BuiltInParameter.RBS_OFFSET_PARAM).AsDouble();
                Document doc = ct.Document;
                _baseLevel = doc.GetElement(ct.get_Parameter(BuiltInParameter.RBS_START_LEVEL_PARAM).AsElementId()) as Level;

                return;
            }

            return;
        }
    }
}
