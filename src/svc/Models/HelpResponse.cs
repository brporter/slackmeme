using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service.Models
{
    public class HelpResponse
        : BasicResponse
    {
        public HelpResponse()
        {
            base.response_type = "ephemeral";
            base.text =
                "To generate a meme for the current channel, type '/meme <memetype>:<meme text>' and I'll generate and insert the meme for you.\nI know about the following memes:\n   - Success Kid (sk)\n   - All The Things (att)\n   - Dwight Schrute (dwight)\n   - I Don't Always (ida)\n   - Doge (doge)\n";
        }
    }
}
