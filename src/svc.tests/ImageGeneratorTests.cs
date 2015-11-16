using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BryanPorter.SlackMeme.Service.Tests
{
    public class ImageGeneratorTests
    {
        [Fact]
        public void CalculateOptimumFontSizeScaleTest()
        {
            const string text = "this is a test of a long string";

            var img = new Bitmap(400, 400);
            var rect = new RectangleF(0, 0, 400, 200);
            var fmt = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            var generator = new ImageGenerator(null);
            var result = generator.CalculateOptimumFontSize(img, rect, fmt, text);

            Assert.Equal(40, result);
        }
    }
}
