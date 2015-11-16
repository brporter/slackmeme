using System.Drawing;

namespace BryanPorter.SlackMeme.Service
{
    public interface IImageGenerator
    {
        Image GenerateImage(string imageKey, string topText, string bottomText);
        float CalculateOptimumFontSize(Image image, RectangleF boundingRectangle, StringFormat format, string text);
    }
}