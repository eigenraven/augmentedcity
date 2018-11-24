using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GTFS;
using GTFS.IO;
using GTFS.Entities;
using GTFS.Entities.Enumerations;
using GTFS.Fields;

namespace backend.CityModel
{
    public class GtfsModel : BaseModel
    {
        public GtfsModel(String path)
        {
            //var ATrips = new List<string>();
            Console.WriteLine("Building GTFS model...");
            var reader = new GTFSReader<GTFSFeed>();
            using (var sources = new GTFSDirectorySource(new DirectoryInfo(path)))
            {
                var feed = reader.Read(sources);
                var stopmap = new Dictionary<string, Entities.Stop>();
                foreach (var stop in feed.Stops)
                {
                    Entities.Location loc = Entities.Location.FromGPS(stop.Latitude, stop.Longitude);
                    var stp = new Entities.Stop() { Name = stop.Name, Location = loc };
                    Stops.Add(stp);
                    stopmap.Add(stop.Id, stp);
                }
                var tripset = from t in feed.Trips group t by t.RouteId;
                var stoptset = from s in feed.StopTimes group s by s.TripId;
                foreach (var route in feed.Routes)
                {
                    Entities.Route R = new Entities.Route();
                    var trips = tripset.Where(t => t.Key == route.Id).Single();
                    var trip = trips.ElementAt(trips.Count() / 2);
                    var stopts = stoptset.Where(s => s.Key == trip.Id).Single();
                    R.Name = route.ShortName;
                    switch (route.Type)
                    {
                        default: R.Type = Entities.Route.Types.Bus; break;
                    }
                    var PS = stopts.First();
                    foreach (var S in stopts)
                    {
                        Entities.Route.StopData sd = new Entities.Route.StopData();
                        sd.SecondsFromPrevious = S.ArrivalTime.TotalSeconds - PS.DepartureTime.TotalSeconds;
                        sd.WaitSeconds = S.DepartureTime.TotalSeconds - S.ArrivalTime.TotalSeconds;
                        sd.StopName = stopmap.GetValueOrDefault(S.StopId).Name;
                        R.Stops.Add(sd);
                        PS = S;
                    }
                    Routes.Add(R);
                    //ATrips.Add(trip.Id);
                }
            }
            //File.WriteAllLines("gtfs/gfilter-tripids.txt", ATrips);
            Console.WriteLine("Done building GTFS model");
        }
    }
}