using System;
using GTFS;
using GTFS.Entities;
using GTFS.Fields;

namespace backend.CityModel
{
    public class GtfsModel
    {
        public GtfsModel(String path)
        {
            var reader = new GTFSReader<GTFSFeed>();
            using (var sources = new GTFSDirectorySource(new DirectoryInfo(path)))
            {
                var feed = reader.Read(sources);
                //
            }
        }
    }
}