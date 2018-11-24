using System;
using System.Collections;
using System.Collections.Generic;
using GTFS;
using GTFS.Entities;
using GTFS.Fields;

namespace backend.CityModel
{
    public class GtfsModel
    {
        List<Entities.Stop> Stops = new List<Entities.Stop>();

        public GtfsModel(String path)
        {
            var reader = new GTFSReader<GTFSFeed>();
            using (var sources = new GTFSDirectorySource(new DirectoryInfo(path)))
            {
                var feed = reader.Read(sources);
                {
                    var stopMap = feed.StopMap;
                    //
                }
            }
        }
    }
}