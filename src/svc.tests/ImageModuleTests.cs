using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Testing;
using Xunit;
using Xunit.Abstractions;

namespace BryanPorter.SlackMeme.Service.Tests
{
    public class ImageModuleTests
    {
        const string ResponseType = "in_channel";
        const string ResponseText = "GENERATE ALL THE MEMES";
        const string ImageUrl = "http://localhost/images/reference-foo-bar.jpg.jpg";
        private const string Title = "foo bar";
        
        [Fact]
        public void GetTest()
        {
            var browser = new Browser(cfg =>
            {
                cfg.Module<ImageModule>();
                cfg.Dependency<IRootPathProvider>(new DefaultRootPathProvider());
                cfg.Dependency<ICommandParser>(new CommandParser());
                cfg.Dependency<IBlobStore>(new MockedImageStore());
                cfg.Dependency<IImageGenerator>(new ImageGenerator(new MockedImageProvider()));
            });

            var result = browser.Post("/image/", context =>
            {
                context.FormValue("text", "reference:foo\\bar");
            });

            var model = result.Body.DeserializeJson<MockedImageResponse>();


            Assert.NotNull(result);
            Assert.Equal(ResponseText, model.text);
            Assert.Equal(ResponseType, model.response_type);
            Assert.NotNull(model.attachments);
            Assert.Equal(1, model.attachments.Length);
            Assert.Equal(ImageUrl, model.attachments[0].image_url);
            Assert.Equal(Title, model.attachments[0].title);
        }
    }
}
