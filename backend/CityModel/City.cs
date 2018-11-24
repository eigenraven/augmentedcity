namespace backend.CityModel
{
    public class City
    {
        public GtfsModel GtfsModel { get; private set; }

        public void Configure(Config cfg)
        {
            Entities.Location.InitializeCenter(cfg.GpsCenter[0], cfg.GpsCenter[1]);
            GtfsModel = new GtfsModel(cfg.GtfsPath);
        }
    }
}