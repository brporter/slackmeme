using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BryanPorter.SlackMeme.Service.Models;

namespace BryanPorter.SlackMeme.Service
{
    public interface ICommandParser
    {
        bool TryParse(string input, out Command parsedCommand);
    }

    public class CommandParser : ICommandParser
    {
        const char PreambleSeperator = ':';
        const char LineSeperator = '\\';

        public bool TryParse(string input, out Command parsedCommand)
        {
            parsedCommand = null;

            if (string.IsNullOrWhiteSpace(input))
            {
                // No input. Early return.
                return false;
            }

            if (input.IndexOf(':') == -1)
            {
                // No preamble specified at all. Early return.
                return false;
            }

            if (string.Compare(input, Command.HelpConstant, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Help request. Early return.
                parsedCommand = Command.HelpCommand;
                return true;
            }
            
            var text = string.Empty;
            var topLine = string.Empty;
            var bottomLine = string.Empty;

            var preambleSeperatorIndex = input.IndexOf(PreambleSeperator);
            var preamble = input.Substring(0, preambleSeperatorIndex);
            var textStartIndex = preambleSeperatorIndex + 1; // Don't include the seperator itself

            if (preambleSeperatorIndex < input.Length)
            {
                // There is text after the preamble
                text = input.Substring(
                    textStartIndex,
                    input.Length - (textStartIndex)
                );
            }

            var lineSeperatorIndex = input.IndexOf(LineSeperator);

            if (lineSeperatorIndex != -1)
            {
                // Input contains a line seperator.
                topLine = text.Substring(0, lineSeperatorIndex);

                var bottomLineStartIndex = lineSeperatorIndex + 1;

                if (bottomLineStartIndex < text.Length)
                {
                    bottomLine = text.Substring(bottomLineStartIndex, text.Length - bottomLineStartIndex);
                }
            }

            parsedCommand = new Command(preamble, topLine, bottomLine);

            return true;
        }
    }
}
