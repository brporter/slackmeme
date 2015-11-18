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
        
        [Fact]
        public void PostTest()
        {
            const string imageResponseType = "in_channel";
            const string imageResponseText = "GENERATE ALL THE MEMES";
            const string imageUrl = "http://localhost/images/reference-foo-bar.jpg.jpg";
            const string imageTitle = "foo bar";

            var browser = new Browser(cfg =>
            {
                cfg.Module<ImageModule>();
                cfg.Dependency<IRootPathProvider>(new DefaultRootPathProvider());
                cfg.Dependency<ICommandParser>(new CommandParser());
                cfg.Dependency<IBlobStore>(new MockedImageStore("reference"));
                cfg.Dependency<IImageGenerator>(new ImageGenerator(new MockedImageProvider()));
            });

            var result = browser.Post("/image/", context =>
            {
                context.FormValue("text", "reference:foo\\bar");
            });

            var model = result.Body.DeserializeJson<MockedImageResponse>();
            
            Assert.NotNull(model);
            Assert.NotNull(model.attachments);
            Assert.Equal(imageResponseText, model.text);
            Assert.Equal(imageResponseType, model.response_type);
            Assert.Equal(1, model.attachments.Length);
            Assert.Equal(imageUrl, model.attachments[0].image_url);
            Assert.Equal(imageTitle, model.attachments[0].title);
        }

        [Theory]
        [InlineData("invalid:")]
        [InlineData("completely invalid")]
        [InlineData(null)]
        public void PostInvalidMemeTypeRequestTest(string text)
        {
            const string unknownResponseText = "Sorry! I don't know what that means! \n\nTo generate a meme for the current channel, type '/meme <memetype>:<meme text>' and I'll generate and insert the meme for you.\nI know about the following memes:\n   - Success Kid (sk)\n   - All The Things (att)\n   - Dwight Schrute (dwight)\n   - I Don't Always (ida)\n   - Doge (doge)\n   - Yoda (yoda1)\n   - Thinkin' Yoda (yoda2)\n";
            const string responseType = "ephemeral";

            var browser = new Browser(cfg =>
            {
                cfg.Module<ImageModule>();
                cfg.Dependency<IRootPathProvider>(new DefaultRootPathProvider());
                cfg.Dependency<ICommandParser>(new CommandParser());
                cfg.Dependency<IBlobStore>(new MockedImageStore("not_invalid"));
                cfg.Dependency<IImageGenerator>(new ImageGenerator(new MockedImageProvider()));
            });

            var result = browser.Post("/image/", context =>
            {
                if (text != null)
                    context.FormValue("text", text);
            });
            
            var model = result.Body.DeserializeJson<Models.UnknownResponse>();

            Assert.NotNull(model);
            Assert.Equal(responseType, model.response_type);
            Assert.Equal(unknownResponseText, model.text);
            Assert.Null(model.attachments);
        }

        [Fact]
        public void PostHelpRequestTest()
        {
            const string helpResponseText = "To generate a meme for the current channel, type '/meme <memetype>:<meme text>' and I'll generate and insert the meme for you.\nI know about the following memes:\n   - Success Kid (sk)\n   - All The Things (att)\n   - Dwight Schrute (dwight)\n   - I Don't Always (ida)\n   - Doge (doge)\n   - Yoda (yoda1)\n   - Thinkin' Yoda (yoda2)\n";
            const string responseType = "ephemeral";

            var browser = new Browser(cfg =>
            {
                cfg.Module<ImageModule>();
                cfg.Dependency<IRootPathProvider>(new DefaultRootPathProvider());
                cfg.Dependency<ICommandParser>(new CommandParser());
                cfg.Dependency<IBlobStore>(new MockedImageStore("not_invalid"));
                cfg.Dependency<IImageGenerator>(new ImageGenerator(new MockedImageProvider()));
            });

            var result = browser.Post("/image/", context =>
            {
                context.FormValue("text", "help");
            });

            var model = result.Body.DeserializeJson<Models.HelpResponse>();

            Assert.NotNull(model);
            Assert.Equal(responseType, model.response_type);
            Assert.Equal(helpResponseText, model.text);
            Assert.Null(model.attachments);
        }
    }
}
