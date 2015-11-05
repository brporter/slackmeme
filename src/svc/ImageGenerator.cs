using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service
{
    // Generates our meme images
    public class ImageGenerator
    {
        readonly static StringFormat TopFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near };
        readonly static StringFormat BtmFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far };

        const int StartingFontSize = 24;
        const int FontSizeIncrement = 8;

        public static Image GetImage(string rootPath, string imageKey, string topText, string bottomText)
        {
            var imagePath = System.IO.Path.Combine(rootPath, "images", string.Format("{0}.jpg", imageKey));

            if (!System.IO.File.Exists(imagePath))
                return null;

            var img = Image.FromFile(imagePath);

            using (var g = Graphics.FromImage(img))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                var topRectangle = new RectangleF(0, 0, img.Width, img.Height/2);
                var bottomRectangle = new RectangleF(0, img.Height/2, img.Width, img.Height/2);
                
                var topPath = new GraphicsPath();
                var btmPath = new GraphicsPath();

                DrawString(g, topText.ToUpper(), topRectangle, TopFormat);
                DrawString(g, bottomText.ToUpper(), bottomRectangle, BtmFormat);

                g.Flush();
            }

            return img;
        }

        private static void DrawString(Graphics g, string text, RectangleF rectangle, StringFormat format)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                SizeF size = SizeF.Empty;
                Font font = null;

                while (size.Height < rectangle.Size.Height &&
                    size.Width < rectangle.Size.Width)
                {
                    font = font == null
                        ? new Font(FontFamily.GenericSansSerif, StartingFontSize, FontStyle.Bold)
                        : new Font(FontFamily.GenericSansSerif, font.Size + FontSizeIncrement, FontStyle.Bold);

                    size = g.MeasureString(text, font, rectangle.Size, format);
                }

                var path = new GraphicsPath();
                path.AddString(text, FontFamily.GenericSansSerif, (int)FontStyle.Bold, font.Size, rectangle, format);
                g.DrawPath(Pens.Black, path);
                g.FillPath(Brushes.White, path);
            }
        }
    }
}
