using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nancy;

namespace BryanPorter.SlackMeme.Service
{
    public class ImageModule
        : Nancy.NancyModule
    {
        const char LineSeperator = '\\';

        public ImageModule(IRootPathProvider rootPathProvider)
        {
            Get["/image/{imageKey}"] = (parameters) =>
            {
                var text = Request.Query.text.HasValue ? (string)Request.Query.text : string.Empty;
                string topText = string.Empty, bottomText = string.Empty;

                if (text.IndexOf(LineSeperator) != -1)
                {
                    var parts = text.Split(LineSeperator);

                    if (parts.Length > 1)
                    {
                        topText = parts[0];
                        bottomText = parts[1];
                    }
                }
                else
                {
                    topText = text;
                }

                var img = ImageGenerator.GetImage(rootPathProvider.GetRootPath(), parameters["imageKey"], topText,
                    bottomText);

                if (img != null)
                {
                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Jpeg);
                    ms.Seek(0, SeekOrigin.Begin);

                    return Response.FromStream(ms, "image/jpeg");
                }

                return HttpStatusCode.InternalServerError;
            };

            Post["/image/"] = _ =>
            {
                var text = Request.Form.text.HasValue ? Request.Form.text.ToString() : "dwight:Next+time\\say+something";
                string[] parts = text.Split(':');
                
                if (parts.Length < 2)
                {
                    var result = new
                    {
                        text = "Sorry! I don't know what that means.",
                        attachments = new[]
                        {
                            new
                            {
                                text =
                                    "To generate a meme for the current channel, type '/meme <memetype>:<meme text>' and I'll generate and insert the meme for you.\nI know about the following memes:\n   * Success Kid (sk)\n   * All The Things (att)\n   * Dwight Schrute (dwight)\n   * I Don't Always (ida)\n   * Doge (doge)\n"
                            }
                        }
                    };

                    return Response.AsJson(result);
                }

                string imageKey = parts[0];
                string memeText = parts[1];

                var r = new
                {
                    response_type = "in_channel",
                    text = "GENERATE ALL THE MEMES",
                    attachments =
                        new[]
                        {
                            new
                            {
                                image_url =
                                    string.Format("https://bpslackmeme.azurewebsites.net/image/{0}?text={1}", imageKey,
                                        Nancy.Helpers.HttpUtility.UrlEncode(memeText)),
                                title = text
                            }
                        }
                };
                
                return Response.AsJson(r, HttpStatusCode.OK);
            };
        }

        private IEnumerable<string> GetImageKeys(string rootPath)
        {
            var imagesPath = Path.Combine(rootPath, "images");
            var images = Directory.GetFiles(imagesPath, "*.jpg");

            return images.Select(Path.GetFileNameWithoutExtension);
        }
    }
}
