namespace backend.CityModel
{
    public class City
    {
        public GtfsModel GtfsModel { get; private set; }

        public void Configure(Config cfg)
        {
            GtfsModel = new GtfsModel(cfg.GtfsPath);
        }
    }
}