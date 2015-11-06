using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service.Models
{
    public class ImageResponse
        : BasicResponse
    {
        const string ResponseType = "in_channel";
        const string ResponseText = "GENERATE ALL THE MEMES";

        public ImageResponse(string imageKey, string imageText)
        {
            base.response_type = ResponseType;
            base.text = ResponseText;

            base.attachments = new[]
            {
                new BasicAttachment()
                {
                    image_url = string.Format("https://bpslackmeme.azurewebsites.net/image/{0}?text={1}", imageKey,
                        Nancy.Helpers.HttpUtility.UrlEncode(imageText)),
                    title = imageText
                }
            };
        }
    }
}
