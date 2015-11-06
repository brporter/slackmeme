using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service.Models
{
    public class UnknownResponse
        : HelpResponse
    {
        public UnknownResponse()
        {
            base.text = "Sorry! I don't know what that means! \n\n" + base.text;
        }
    }
}
