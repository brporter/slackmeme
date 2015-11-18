using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BryanPorter.SlackMeme.Service.Models;
using Nancy;
using Nancy.Responses;

namespace BryanPorter.SlackMeme.Service
{
    public class ImageModule
        : Nancy.NancyModule
    {
        static readonly UnknownResponse UnknownResponse = new UnknownResponse();
        static readonly HelpResponse HelpResponse = new HelpResponse();

        public ImageModule(IRootPathProvider rootPathProvider, ICommandParser commandParser, IBlobStore store, IImageGenerator imageGenerator)
        {
            Post["/image/"] = _ =>
            {
                Command c = null;

                if (!commandParser.TryParse(Request.Form.text.HasValue ? (string)Request.Form.text : string.Empty, out c))
                {
                    // Invalid input. Early return.
                    return Response.AsJson(UnknownResponse);
                }

                if (string.Compare(c.Preamble, Command.HelpConstant, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // Help request. Early return.
                    return Response.AsJson(HelpResponse);
                }

                var imageId = string.Format("{0}-{1}-{2}.jpg",
                    c.Preamble,
                    Nancy.Helpers.HttpUtility.UrlEncode(c.TopLine ?? string.Empty),
                    Nancy.Helpers.HttpUtility.UrlEncode(c.BottomLine ?? string.Empty));

                if (!store.Exists(imageId))
                {
                    var img = imageGenerator.GenerateImage(c.Preamble, c.TopLine, c.BottomLine);

                    if (img != null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            img.Save(ms, ImageFormat.Jpeg);
                            ms.Seek(0, SeekOrigin.Begin);

                            store.Store(imageId, ms);
                        }
                    }
                    else
                    {
                        // We failed to generate an image; send unknown response
                        return Response.AsJson(new Models.UnknownResponse());
                    }
                }

                return Response.AsJson(
                    new Models.ImageResponse(store.GetUri(imageId).ToString(), 
                    string.Format("{0} {1}", c.TopLine, c.BottomLine)));
            };
        }
    }
}
