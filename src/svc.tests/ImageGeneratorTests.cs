using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BryanPorter.SlackMeme.Service.Tests
{
    public class ImageGeneratorTests
    {
        // MD5 Hash of our reference image, w/o processing
        static readonly byte[] ReferenceHash = { 43, 114, 184, 113, 43, 99, 45, 35, 131, 182, 78, 207, 166, 61, 155, 167 };

        // Expected MD5 hash of our processed image
        static readonly byte[] ProcessedHash = { 101, 180, 58, 124, 79, 20, 148, 203, 185, 65, 66, 173, 253, 96, 237, 2 };

        [Theory]
        [InlineData("this is a test of a long string", 40)]
        [InlineData(null, 16)]
        public void CalculateOptimumFontSizeScaleTest(string text, int expectedSize)
        {
            var img = new Bitmap(400, 400);
            var rect = new RectangleF(0, 0, 400, 200);
            var fmt = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Near
            };

            var generator = new ImageGenerator(null);
            var result = generator.CalculateOptimumFontSize(img, rect, fmt, text);

            Assert.Equal(expectedSize, result);
        }

        [Fact]
        public void GenerateImageTest()
        {
            var i = new MockedImageProvider();
            var g = new ImageGenerator(i);

            var img = g.GenerateImage("reference", "topText", "bottomText");

            byte[] resultHash = null;

            using (var s = new MemoryStream())
            {
                img.Save(s, ImageFormat.Jpeg);
                s.Position = 0;
                s.Seek(0, SeekOrigin.Begin);

                using (var md5 = MD5.Create())
                {
                    resultHash = md5.ComputeHash(s);
                }
            }

            Assert.Equal(ProcessedHash, resultHash);
        }

        [Fact]
        public void GenerateNullImageTest()
        {
            var i = new MockedImageProvider();
            var g = new ImageGenerator(i);

            var img = g.GenerateImage(null, null, null);

            Assert.Null(img);
        }
    }
}
