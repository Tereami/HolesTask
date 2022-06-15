using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;


namespace HolesTask
{
    public class LocationInfo
    {
        public double X;
        public double Y;
        public double Z;

        public double B;
        public double L;
        public double H;

        public double Angle;

        public Level BaseLevel;
        public double LevelElev;
        public double OffsetFromLevel;

        public LocationInfo(FamilyInstance fi)
        {
            LocationPoint lp = fi.Location as LocationPoint;
            XYZ p = lp.Point;
            X = p.X;
            Y = p.Y;
            Z = p.Z;

            B = fi.get_Parameter(new Guid(StaticPlaceTask.widthParamGuid)).AsDouble();
            H = fi.get_Parameter(new Guid(StaticPlaceTask.heigthParamGuid)).AsDouble();
            L = fi.get_Parameter(new Guid(StaticPlaceTask.thicknessParamGuid)).AsDouble();

            Angle = lp.Rotation;

            BaseLevel = fi.Document.GetElement(fi.LevelId) as Level;
        }

        public XYZ GetPlacePoint()
        {
            XYZ p = new XYZ(X, Y, Z);
            return p;
        }

        public Level FindLevel(Document doc)
        {
            List<Level> levels = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .WhereElementIsNotElementType()
                .Cast<Level>()
                .Where(l => Math.Round(l.Elevation,5) == Math.Round(LevelElev, 5))
                .ToList();

            if (levels.Count == 0) return null;

            Level lev = levels.First();

            BaseLevel = lev;

            double offset = 0;
            if (BaseLevel != null)
            {
                offset = Z - BaseLevel.Elevation;
            }
            else
            {
                offset = Z - LevelElev;
            }
            OffsetFromLevel = offset;

            return BaseLevel;
        }
       

        public LocationInfo(double x, double y, double z, double b, double h, double l, double angle, double level)
        {
            X = x;
            Y = y;
            Z = z;
            B = b;
            H = h;
            L = l;
            Angle = angle;
            LevelElev = level;
        }

    }
}
