namespace backend.CityModel
{
    public class City
    {
        public Config Config { get; private set; }
        public GtfsModel GtfsModel { get; private set; }

        public void Configure(Config cfg)
        {
            this.Config = cfg;
            Entities.Location.InitializeCenter(cfg.GpsCenter[0], cfg.GpsCenter[1]);
            GtfsModel = new GtfsModel(cfg.GtfsPath);
        }
    }
}