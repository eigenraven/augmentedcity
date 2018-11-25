import std.experimental.all;

struct Cell
{
    int id;
    int size;
    double lat, lon;
}

void main()
{
    const fullFile = readText("fullgrid.geojson");
    auto root = parseJSON(fullFile, JSONOptions.none);
    auto cells = appender!(Cell[]);
    foreach (ref JSONValue node; root["features"].array)
    {
        if (node["type"].str != "Feature")
            continue;
        Cell C;
        C.id = cast(int) node["properties"]["ID"].integer();
        C.size = cast(int) node["properties"]["size"].integer();
        C.lat = 0.0;
        C.lon = 0.0;
        foreach (ulong i, ref JSONValue cval; node["geometry"]["coordinates"][0][0])
        {
            if (i == 0)
                continue;
            C.lat += cval[1].floating;
            C.lon += cval[0].floating;
        }
        C.lat /= 4.0;
        C.lon /= 4.0;
        double Dlat = fabs(C.lat - 60.164926);
        double Dlon = fabs(C.lon - 24.939405);
        if (Dlat > 0.12 || Dlon > 0.22)
        {
            continue;
        }
        cells ~= C;
    }
    foreach (ref Cell C; cells.data)
    {
        writefln!"%d,%d,%.6f,%.6f"(C.id, C.size, C.lat, C.lon);
    }
}
