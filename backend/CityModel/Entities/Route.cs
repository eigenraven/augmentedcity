using System.Collections.Generic;
namespace backend.CityModel.Entities
{
    public class Route
    {
        public enum Types
        {
            LightRail,
            UndergroundRail,
            Rail,
            Bus,
            Ferry,
            StreetCableCar,
            Gondola,
            Funicular, // very steep rails
        }

        public class StopData
        {
            public string StopName { get; set; }
            public float WaitSeconds { get; set; }
            public float SecondsFromPrevious { get; set; }
        }

        public string Name { get; set; }
        public Types Type { get; set; }
        public List<StopData> Stops { get; set; } = new List<StopData>();
    }
}