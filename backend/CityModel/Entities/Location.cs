using System;

namespace backend.CityModel.Entities
{
    public struct Location
    {
        static readonly double EarthA = 6_378_137.0; // metres
        static readonly double EarthB = 6_356_752.3;
        static readonly double EarthR = (EarthA + EarthB) / 2.0;

        static double CenterLatitude, CenterLongitude;
        static Location CenterPosition; // in ECEF coordinates
        float XOffset, YOffset;

        public static void InitializeCenter(double Latitude, double Longitude)
        {
            CenterLatitude = 0.0;
            CenterLongitude = 0.0;
            var O = FromGPS(Latitude, Longitude);
            CenterPosition = O;
            CenterLatitude = Latitude;
            CenterLongitude = Longitude;
        }

        public static Location FromGPS(double Latitude, double Longitude)
        {
            double sq(double u) { return u * u; }
            double x = 0.0f, y = 0.0f;

            // https://en.wikipedia.org/wiki/Geographic_coordinate_conversion#From_geodetic_to_ECEF_coordinates
            double CosPhi = Math.Cos(Latitude);
            double SinPhi = Math.Sin(Latitude);
            double CosLam = Math.Cos(Longitude);
            double SinLam = Math.Sin(Longitude);
            double N = sq(EarthA) / Math.Sqrt(sq(EarthA * CosPhi) + sq(EarthB * SinPhi));
            double X = (N + EarthR) * CosPhi * CosLam;
            double Y = (N + EarthR) * CosPhi * SinLam;
            double Z = (sq(EarthB / EarthA) * N + EarthR) * SinPhi;

            // TODO: Project X,Y,Z from ECEF to a local 2d plane

            return new Location { XOffset = x, YOffset = y };
        }
    }
}