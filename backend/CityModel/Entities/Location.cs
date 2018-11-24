using System;
using System.Linq;

namespace backend.CityModel.Entities
{
    public struct Location
    {
        static double sq(double u) { return u * u; }
        static double dot(double[] a, double[] b) { return a.Zip(b, (x, y) => x * y).Sum(); }

        static readonly double EarthA = 6_378.137_0; // kilometres
        static readonly double EarthB = 6_356.752_3;
        static readonly double EarthR = (EarthA + EarthB) / 2.0;

        static double CenterLatitude, CenterLongitude;
        static double[] NorthV3, EastV3;
        static double[] CenterPosition; // in ECEF coordinates
        public float XOffset { get; set; }
        public float YOffset { get; set; }

        public Location(float x, float y)
        {
            XOffset = x;
            YOffset = y;
        }

        public static void InitializeCenter(double Latitude, double Longitude)
        {
            CenterLatitude = 0.0;
            CenterLongitude = 0.0;

            double CosPhi = Math.Cos(Latitude);
            double SinPhi = Math.Sin(Latitude);
            double CosLam = Math.Cos(Longitude);
            double SinLam = Math.Sin(Longitude);
            double N = sq(EarthA) / Math.Sqrt(sq(EarthA * CosPhi) + sq(EarthB * SinPhi));
            double X = (N + EarthR) * CosPhi * CosLam;
            double Y = (N + EarthR) * CosPhi * SinLam;
            double Z = (sq(EarthB / EarthA) * N + EarthR) * SinPhi;

            CenterPosition = new double[] { X, Y, Z };

            NorthV3 = new double[] {
                (N + EarthR) * (-SinPhi) * CosLam,
                (N + EarthR) * (-SinPhi) * SinLam,
                (sq(EarthB / EarthA) * N + EarthR) * CosPhi
            };
            double Len = (from c in NorthV3 select c * c).Sum();
            NorthV3 = (from c in NorthV3 select c / Len).ToArray();

            EastV3 = new double[] {
                (N + EarthR) * CosPhi * (-SinLam),
                (N + EarthR) * CosPhi * CosLam,
                0.0
            };
            Len = (from c in EastV3 select c * c).Sum();
            EastV3 = (from c in EastV3 select c / Len).ToArray();

            CenterLatitude = Latitude;
            CenterLongitude = Longitude;
        }

        public static Location FromGPS(double Latitude, double Longitude)
        {
            double x = 0.0f, y = 0.0f;

            // https://en.wikipedia.org/wiki/Geographic_coordinate_conversion#From_geodetic_to_ECEF_coordinates
            double CosPhi = Math.Cos(Latitude);
            double SinPhi = Math.Sin(Latitude);
            double CosLam = Math.Cos(Longitude);
            double SinLam = Math.Sin(Longitude);
            double N = sq(EarthA) / Math.Sqrt(sq(EarthA * CosPhi) + sq(EarthB * SinPhi));
            double[] P = new double[] {
                (N + EarthR) * CosPhi * CosLam,
                (N + EarthR) * CosPhi * SinLam,
                (sq(EarthB / EarthA) * N + EarthR) * SinPhi
            };

            for (int i = 0; i < 3; i++) P[i] -= CenterPosition[i];
            x = dot(P, EastV3);
            y = dot(P, NorthV3);

            return new Location { XOffset = (float)x, YOffset = (float)y };
        }
    }
}