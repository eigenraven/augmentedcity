using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using backend.CityModel.Entities;
using backend.CityModel;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/city")]
    public class CityController : ControllerBase
    {
        private readonly CityModel.City _Model;
        private ColorSpaceConverter _ColorSpaceConverter = new ColorSpaceConverter();
        private JpegEncoder _JpegEncoder = new JpegEncoder();

        public CityController(CityModel.City model)
        {
            _Model = model;
            _JpegEncoder.Quality = 100;
            _JpegEncoder.Subsample = JpegSubsample.Ratio444;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return _Model.Config.Name;
        }

        [HttpGet("stops")]
        public ActionResult<IEnumerable<Stop>> GetStops()
        {
            return _Model.GtfsModel.Stops;
        }

        [HttpGet("routes")]
        public ActionResult<IEnumerable<Route>> GetRoutes()
        {
            return _Model.GtfsModel.Routes;
        }

        [HttpGet("heatmap/{index}.jpg")]
        public IActionResult GetHeatmap(int index)
        {
            if (index < 0 || index > 23)
            {
                return NotFound();
            }
            var img = new Image<Rgba32>(64, 64);
            var mgm = _Model.MovingGridModel;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int closestI = 0;
                    float clsD = 1.0e6f;
                    MovingGridModel.CellDef CD;
                    float X, Y;
                    for (int i = 1; i < mgm.TimeSeries.GetLength(1); i++)
                    {
                        CD = mgm.CellDefs[i];
                        X = (CD.Location.XOffset + 0.5f) * img.Width;
                        Y = (CD.Location.YOffset + 0.5f) * img.Height;
                        float ND = (X - x) * (X - x) + (Y - y) * (Y - y);
                        if (ND < clsD)
                        {
                            clsD = ND;
                            closestI = i;
                        }
                    }
                    CD = mgm.CellDefs[closestI];
                    X = (CD.Location.XOffset + 0.5f) * img.Width;
                    Y = (CD.Location.YOffset + 0.5f) * img.Height;
                    X = Math.Clamp(X, 0, img.Width);
                    Y = Math.Clamp(Y, 0, img.Height);
                    float hue = (float)Math.Pow(mgm.TimeSeries[index, closestI] / (double)(0.8 * mgm.MaxActivity), 0.7);
                    hue = Math.Clamp(hue, 0, 1);
                    float rhue = 120.0f - hue * 140.0f;
                    Rgb rgb = _ColorSpaceConverter.ToRgb(new Hsv(rhue, rhue < 50.0f ? 0.9f : 0.5f, 1.0f));
                    //Rgb rgb = new Rgb(0.0f, hue, 0.0f);
                    img[x, y] = new Rgba32(rgb.ToVector3());
                }
            }

            var ms = new MemoryStream();
            img.SaveAsJpeg(ms, _JpegEncoder);
            byte[] arr = ms.ToArray();
            ms.Close();
            return File(arr, "image/jpeg");
        }
    }
}