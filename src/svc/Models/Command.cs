using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BryanPorter.SlackMeme.Service.Models
{
    public class Command
    {
        public const string HelpConstant = "help";
        public static readonly Command HelpCommand = new Command() { Preamble = HelpConstant, BottomLine = null, TopLine = null };

        private Command()
        { }

        public Command(string preamble, string topLine, string bottomLine)
        {
            Preamble = preamble;
            TopLine = topLine;
            BottomLine = bottomLine;
        }

        public string Preamble { get; private set; }

        public string TopLine { get; private set; }

        public string BottomLine { get; private set; }
    }
}
