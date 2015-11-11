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

                var topSize = CalculateOptimumFontSize(img, topRectangle, TopFormat, topText);
                var btmSize = CalculateOptimumFontSize(img, bottomRectangle, BtmFormat, bottomText);
                
                var path = new GraphicsPath();

                path.AddString(topText, FontFamily.GenericSansSerif, (int) FontStyle.Bold, topSize, topRectangle,
                    TopFormat);
                path.AddString(bottomText, FontFamily.GenericSansSerif, (int) FontStyle.Bold, btmSize, bottomRectangle,
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

            Font font = null;
            var size = SizeF.Empty;
            var safetyLoopCounter = (uint) 0;

            using (var g = Graphics.FromImage(image))
            {

                while (safetyLoopCounter < 40)
                {
                    safetyLoopCounter++;

                    font = font == null
                        ? new Font(FontFamily.GenericSansSerif, StartingFontSize, FontStyle.Bold)
                        : new Font(FontFamily.GenericSansSerif, font.Size + FontSizeIncrement, FontStyle.Bold);

                    var testSize = g.MeasureString(text, font, boundingRectangle.Size, format);

                    if (size.Height < boundingRectangle.Size.Height &&
                        size.Width < boundingRectangle.Size.Width)
                    {
                        size = testSize;
                    }
                    else
                    {
                        // We've exceeded bounds, back it out.
                        break;
                    }
                }
            }

            return font.Size;
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
