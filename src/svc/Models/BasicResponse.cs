using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service.Models
{
    public class BasicResponse
    {
        public string response_type { get; set; }

        public string text { get; set; }

        public BasicAttachment[] attachments { get; set; }
    }

    public class BasicAttachment
    {
        public string image_url { get; set; }
        public string title { get; set; }
        public string text { get; set; }
    }
}
