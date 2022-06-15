using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace HolesTask
{
    public static class Intersection
    {
        public static XYZ GetIntersectionPoint(Line line, Element elem)
        {
            Solid solid = Intersection.GetSolids(elem)[0];

            List<IntersectionResult> lr = new List<IntersectionResult>();
            foreach (Face face in solid.Faces)
            {
                IntersectionResultArray results = new IntersectionResultArray();
                SetComparisonResult res = face.Intersect(line, out results);
                if (res != SetComparisonResult.Overlap) continue;

                IntersectionResult ir = results.get_Item(0);
                lr.Add(ir);
            }

            if (lr.Count > 1)
            {
                XYZ p1 = lr[0].XYZPoint;
                XYZ p2 = lr[1].XYZPoint;
                XYZ p = new XYZ((p1.X + p2.X) / 2, (p1.Y + p2.Y) / 2, (p1.Z + p2.Z) / 2);
                return p;
            }
            if (lr.Count == 1)
            {
                return lr[0].XYZPoint;
            }
            return null;
        }


        public static List<Solid> GetSolids(Element elem)
        {
            GeometryElement geoElem = elem.get_Geometry(new Options());
            List<Solid> solids = GetSolids(geoElem);
            return solids;
        }

        private static List<Solid> GetSolids(GeometryElement geoElem)
        {
            List<Solid> solids = new List<Solid>();

            foreach (GeometryObject geoObj in geoElem)
            {
                if (geoObj is Solid)
                {
                    Solid solid = geoObj as Solid;
                    if (solid == null) continue;
                    if (solid.Volume == 0) continue;
                    solids.Add(solid);
                    continue;
                }
                if (geoObj is GeometryInstance)
                {
                    GeometryInstance geomIns = geoObj as GeometryInstance;
                    GeometryElement instGeoElement = geomIns.GetInstanceGeometry();
                    List<Solid> solids2 = GetSolids(instGeoElement);
                    solids.AddRange(solids2);
                }
            }
            return solids;
        }

    }
}
