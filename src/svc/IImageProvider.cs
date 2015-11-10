using System.Collections.Generic;
using System.Drawing;

namespace BryanPorter.SlackMeme.Service
{
    public interface IImageProvider
    {
        Image GetImage(string imageKey);
        IEnumerable<string> GetImageKeys();
    }
}