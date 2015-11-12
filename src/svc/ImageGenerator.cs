using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service
{
    // Generates our meme images
    public interface IImageGenerator
    {
        Image GenerateImage(string imageKey, string topText, string bottomText);
        float CalculateOptimumFontSize(Image image, RectangleF boundingRectangle, StringFormat format, string text);
    }

    public class ImageGenerator : IImageGenerator
    {
        const int StartingFontSize = 16;
        const int FontSizeIncrement = 8;

        readonly static StringFormat TopFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        readonly static StringFormat BtmFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };

        readonly IImageProvider _imageProvider;

        public ImageGenerator(IImageProvider imageProvider)
        {
            _imageProvider = imageProvider;
        }
        
        public Image GenerateImage(string imageKey, string topText, string bottomText)
        {
            var img = _imageProvider.GetImage(imageKey);

            if (img == null)
                return null;

            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var topRectangle = new RectangleF(0, 0, img.Width, img.Height/2);
                var bottomRectangle = new RectangleF(0, img.Height/2, img.Width, img.Height/2);

                var upperTopText = topText.ToUpper();
                var upperBottomText = bottomText.ToUpper();

                var topSize = CalculateOptimumFontSize(img, topRectangle, TopFormat, upperTopText);
                var btmSize = CalculateOptimumFontSize(img, bottomRectangle, BtmFormat, upperBottomText);
                
                var path = new GraphicsPath();

                path.AddString(upperTopText, FontFamily.GenericSansSerif, (int) FontStyle.Bold, topSize, topRectangle,
                    TopFormat);
                path.AddString(upperBottomText, FontFamily.GenericSansSerif, (int) FontStyle.Bold, btmSize, bottomRectangle,
                    BtmFormat);

                g.DrawPath(Pens.Black, path);
                g.FillPath(Brushes.White, path);

                g.Flush();
            }

            return img;
        }

        public float CalculateOptimumFontSize(Image image, RectangleF boundingRectangle, StringFormat format, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return StartingFontSize;

            const int MaxFontSize = 98;
            const int MinFontSize = 16;
            const int FontSizeAdjustmentInterval = 8;

            float idealFontSize = 0;

            using (var g = Graphics.FromImage(image))
            {
                for (int testFontSize = MinFontSize; testFontSize <= MaxFontSize; testFontSize += FontSizeAdjustmentInterval)
                {
                    var testFont = new Font(FontFamily.GenericSansSerif, testFontSize, FontStyle.Bold);

                    var testSize = g.MeasureString(text, testFont, boundingRectangle.Size, format);

                    if (testSize.Height < boundingRectangle.Size.Height &&
                        testSize.Width < boundingRectangle.Size.Width)
                    {
                        idealFontSize = testFont.Size;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return idealFontSize;
        }

        private static void DrawString(Graphics g, string text, RectangleF rectangle, StringFormat format)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SizeF size = SizeF.Empty;
                Font font = null;

                uint safetyLoopCounter = 0;

                while (safetyLoopCounter < 40)
                {
                    safetyLoopCounter++;

                    font = font == null
                        ? new Font(FontFamily.GenericSansSerif, StartingFontSize, FontStyle.Bold)
                        : new Font(FontFamily.GenericSansSerif, font.Size + FontSizeIncrement, FontStyle.Bold);

                    var testSize = g.MeasureString(text, font, rectangle.Size, format);

                    if (size.Height < rectangle.Size.Height &&
                        size.Width < rectangle.Size.Width)
                    {
                        size = testSize;
                    }
                    else
                    {
                        // We've exceeded bounds, back it out.
                        break;
                    }
                }

                var path = new GraphicsPath();
                path.AddString(text, FontFamily.GenericSansSerif, (int)FontStyle.Bold, font.Size, rectangle, format);
                g.DrawPath(Pens.Black, path);
                g.FillPath(Brushes.White, path);
            }
        }
    }
}
