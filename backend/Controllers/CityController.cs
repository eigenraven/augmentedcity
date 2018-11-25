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
using SixLabors.ImageSharp.Formats.Png;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/city")]
    public class CityController : ControllerBase
    {
        private readonly CityModel.City _Model;
        private PngEncoder _PngEncoder = new PngEncoder();
        private ColorSpaceConverter _ColorSpaceConverter = new ColorSpaceConverter();

        public CityController(IOptions<CityModel.City> model)
        {
            _Model = model.Value;
            _PngEncoder.ColorType = PngColorType.Rgb;
            _PngEncoder.CompressionLevel = 7;
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

        [HttpGet("heatmap/{index}.png")]
        public IActionResult GetHeatmap(int index)
        {
            if (index < 0 || index > 23)
            {
                return NotFound();
            }
            var img = new Image<Rgb24>(256, 256);
            var mgm = _Model.MovingGridModel;

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    img[x, y] = new Rgb24(32, 32, 200);
                }
            }

            for (int i = 0; i < mgm.TimeSeries.GetLength(1); i++)
            {
                MovingGridModel.CellDef CD = mgm.CellDefs[i];
                float X = CD.Location.XOffset * 40.0f;
                float Y = CD.Location.YOffset * 40.0f;
                X = Math.Clamp(X, -img.Width / 2, img.Width / 2 - 1) + img.Width / 2;
                Y = Math.Clamp(Y, -img.Height / 2, img.Height / 2 - 1) + img.Height / 2;
                float hue = (float)mgm.TimeSeries[index, i] / (float)mgm.MaxActivity;
                Rgb rgb = _ColorSpaceConverter.ToRgb(new Hsv(120.0f - hue * 110.0f, 0.8f, 0.9f));
                img[(int)X, (int)Y] = new Rgb24((byte)(rgb.R * 255.0f), (byte)(rgb.G * 255.0f), (byte)(rgb.B * 255.0f));
            }

            var ms = new MemoryStream();
            _PngEncoder.Encode(img, ms);
            byte[] arr = ms.ToArray();
            ms.Close();
            return File(arr, "image/png");
        }
    }
}