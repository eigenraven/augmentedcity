namespace backend.CityModel
{
    public class Config
    {
        public string Name { get; set; }
        public string GtfsPath { get; set; }
        public string GridIdPath { get; set; }
        public string GridDataPath { get; set; }
        public double[] GpsCenter { get; set; }
        public double[] PlanarScale { get; set; }
    }
}