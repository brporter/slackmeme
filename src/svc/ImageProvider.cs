using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace BryanPorter.SlackMeme.Service
{
    public class ImageProvider : IImageProvider
    {
        const string ImageFolder = "images";
        const string ImageFilter = "*.jpg";
        const string ImageFileNameFormat = "{0}.jpg";

        readonly IRootPathProvider _rootPathProvider;

        public ImageProvider(IRootPathProvider rootPathProvider)
        {
            _rootPathProvider = rootPathProvider;
        }

        public Image GetImage(string imageKey)
        {
            var imagePath = Path.Combine(_rootPathProvider.GetRootPath(),
                ImageFolder,
                string.Format(ImageFileNameFormat, imageKey));

            if (!File.Exists(imagePath))
                return null;

            return Image.FromFile(imagePath);
        }

        public IEnumerable<string> GetImageKeys()
        {
            var imageStoragePath = Path.Combine(_rootPathProvider.GetRootPath(), ImageFolder);

            return Directory
                .GetFiles(imageStoragePath, ImageFilter)
                .Select(Path.GetFileNameWithoutExtension);
        }
    }
}
