using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

    public class CommandParser 
        : ICommandParser
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

            if (textStartIndex < input.Length)
            {
                // There is text after the preamble
                text = input.Substring(
                    textStartIndex,
                    input.Length - (textStartIndex)
                    );
            }

            var literal = text.ToLiteral();
            var lines = literal.Split(new[] { LineSeperator });

            topLine = lines.Length >= 1 ? lines[0] : string.Empty;
            bottomLine = lines.Length == 2 ? lines[1] : lines.Length > 2 ? string.Join("\\", lines.Skip(1)) : string.Empty;

            parsedCommand = new Command(preamble, topLine, bottomLine);

            return true;
        }
    }

    internal static class StringExtensions
    {
        public static string ToLiteral(this string input)
        {
            // The Horror: escape sequences in user-supplied input can't be evaluated without
            // replacement in C#. Major pain when a user-supplied backslash literal is your
            // line seperator. *sigh*

            // http://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal

            var literal = new StringBuilder(input.Length);

            foreach (var c in input)
            {
                switch (c)
                {
                    case '\'': literal.Append(@"\'"); break;
                    case '\"': literal.Append("\\\""); break;
                    //case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        if (Char.GetUnicodeCategory(c) != UnicodeCategory.Control)
                        {
                            literal.Append(c);
                        }
                        else
                        {
                            literal.Append(@"\u");
                            literal.Append(((ushort)c).ToString("x4"));
                        }
                        break;
                }
            }

            return literal.ToString();
        }
    }
}
