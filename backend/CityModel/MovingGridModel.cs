using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace backend.CityModel
{
    public class MovingGridModel
    {
        // [time][cell]
        public long[,] TimeSeries { get; private set; }
        public long MaxActivity { get; private set; }
        public List<CellDef> CellDefs { get; private set; } = new List<CellDef>();

        public struct CellDef
        {
            public int Id;
            public int Size;
            public float Latitude, Longitude;
            public Entities.Location Location;
        }

        public MovingGridModel(string idPath, string dataPath)
        {
            Console.WriteLine("Loading MovingGridModel...");
            string[] gridDefs = File.ReadAllLines(idPath);
            var cellDict = new Dictionary<int, int>(); // Id -> index
            foreach (var gridDef in gridDefs)
            {
                var def = gridDef.Trim();
                if (def.Length < 3) { continue; }
                var fields = gridDef.Split(',');
                float lat = float.Parse(fields[2]), lon = float.Parse(fields[3]);
                CellDef CD = new CellDef
                {
                    Id = int.Parse(fields[0]),
                    Size = int.Parse(fields[1]),
                    Latitude = lat,
                    Longitude = lon,
                    Location = Entities.Location.FromGPS(lat, lon)
                };
                cellDict.Add(CD.Id, CellDefs.Count);
                CellDefs.Add(CD);
            }
            string line;
            var dataFile = new StreamReader(dataPath, Encoding.UTF8);
            dataFile.ReadLine();
            TimeSeries = new long[24, CellDefs.Count];
            MaxActivity = 1;
            int lines = 0;
            while ((line = dataFile.ReadLine()) != null)
            {// dominant_zone,time,count
             // 10,16.5.2018 15.00.00,2516
                string[] fields = line.Split(',');
                int zone = int.Parse(fields[0]);
                int cindex;
                if (!cellDict.TryGetValue(zone, out cindex))
                    continue;
                int space = fields[1].IndexOf(' ');
                int hour = int.Parse(fields[1].Substring(space + 1, fields[1].IndexOf('.', space) - space - 1)) % 24;
                long count = long.Parse(fields[2]);
                TimeSeries[hour, cindex] += count;
                if (TimeSeries[hour, cindex] > MaxActivity)
                {
                    MaxActivity = TimeSeries[hour, cindex];
                }
                lines++;
            }
            Console.WriteLine("Done loading MovingGridModel");
            Console.WriteLine("Max activity: " + MaxActivity);
            Console.WriteLine("People scale maximum: " + MaxActivity / 30.0f);
        }
    }
}