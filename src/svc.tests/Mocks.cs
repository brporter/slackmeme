using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Nancy;

namespace BryanPorter.SlackMeme.Service.Tests
{
    public class MockedImageResponse
        : Models.ImageResponse
    {
        public MockedImageResponse()
            : base(null, null)
        {
        }
    }

    public class MockedImageStore
        : IBlobStore
    {
        readonly string _validIdentifier;

        public MockedImageStore(string validIdentifier)
        {
            _validIdentifier = validIdentifier;
        }

        public Uri GetUri(string identifier)
        {
            return new Uri(string.Format("http://localhost/images/{0}.jpg", identifier));
        }

        public bool Exists(string identifier)
        {
            return string.Compare(_validIdentifier, identifier, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public void Store(string identifier, Stream stream)
        {
        }

        public void Delete(string identifier)
        {
        }
    }

    public class MockedImageProvider
        : IImageProvider
    {
        const string ImageKey = "reference";

        public Image GetImage(string imageKey)
        {
            if (String.Compare(ImageKey, imageKey, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return null;
            }

            using (var strm =
                Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("BryanPorter.SlackMeme.Service.Tests.Resources.reference.jpg"))
            {
                return Image.FromStream(strm);
            }
        }

        public IEnumerable<string> GetImageKeys()
        {
            yield return ImageKey;
        }
    }
}