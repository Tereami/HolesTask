using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace HolesTask
{
    public class AngleCalculationResult
    {
        public double HorizontalAngle { get; }
        public double VerticalAngle { get; }
        public double HorizontalAngleDegrees { get; }
        public double VerticalAngleDegrees { get; }

        public AngleCalculationResult(double horizontalAngle, double verticalAngle)
        {
            HorizontalAngle = horizontalAngle;
            VerticalAngle = verticalAngle;
            HorizontalAngleDegrees = horizontalAngle * 180 / Math.PI;
            VerticalAngleDegrees = verticalAngle * 180 / Math.PI;
        }
    }

    public static class StaticAngle
    {
        /// <summary>
        /// Вычисляет угол в горизонтальной плоскости между двумя векторами, заданными точками
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double CalculateAngle(XYZ v1, XYZ v2)
        {
            double cosf = (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z) /
                (
                Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2) + Math.Pow(v1.Z, 2))
                *
                Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2) + Math.Pow(v2.Z, 2))
                );
            double angle = Math.Acos(cosf);

            if (v2.Y < 0 && v2.X > 0) angle = angle * -1;
            if (v2.Y < 0 && v2.X < 0) angle = Math.PI - angle;


            return angle;
        }

        /// <summary>
        /// Вычисляет угол поворота стены в связанном файле
        /// </summary>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static AngleCalculationResult CalculateWallTaskRotateAngle(Wall wall)
        {
            LocationCurve lc = wall.Location as LocationCurve;
            Line wallLine = lc.Curve as Line;

            if (wallLine == null)
            {
                return new AngleCalculationResult(0, 0);
            }

            XYZ v1 = new XYZ(1, 0, 0);

            XYZ v2 = new XYZ(
                wallLine.GetEndPoint(1).X - wallLine.GetEndPoint(0).X,
                wallLine.GetEndPoint(1).Y - wallLine.GetEndPoint(0).Y,
                wallLine.GetEndPoint(1).Z - wallLine.GetEndPoint(0).Z);

            double angle = CalculateAngle(v1, v2);

            AngleCalculationResult acr = new AngleCalculationResult(angle, 0);
            return acr;
        }

        /// <summary>
        /// Вычисляет угол между стеной и элементом инженерной системы
        /// </summary>
        /// <param name="gme"></param>
        /// <param name="wall"></param>
        /// <returns></returns>
        public static AngleCalculationResult CalculateWallMepAngle(GenericMepElement gme, Wall wall)
        {
            double horizAngle = 0;
            double vertAngle = 0;

            LocationCurve lw = wall.Location as LocationCurve;
            Line wallLine = lw.Curve as Line;

            Line mepLine = gme.locationCurve;

            XYZ v1 = new XYZ(
                wallLine.GetEndPoint(1).X - wallLine.GetEndPoint(0).X,
                wallLine.GetEndPoint(1).Y - wallLine.GetEndPoint(0).Y,
                0);

            XYZ v2 = new XYZ(
                mepLine.GetEndPoint(1).X - mepLine.GetEndPoint(0).X,
                mepLine.GetEndPoint(1).Y - mepLine.GetEndPoint(0).Y,
                0);

            horizAngle = CalculateAngle(v1, v2);
            AngleCalculationResult acr = new AngleCalculationResult(horizAngle, vertAngle);
            return acr;
        }
    }
}
