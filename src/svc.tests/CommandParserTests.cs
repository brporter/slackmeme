using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BryanPorter.SlackMeme.Service.Models;
using Xunit;

namespace BryanPorter.SlackMeme.Service.Tests
{
    public class CommandParserTests
    {
        private const string EmptyString = "";

        [Theory]
        [InlineData(null, null, null, null, false)]
        [InlineData("foo", null, null, null, false)]
        [InlineData(Command.HelpConstant, Command.HelpConstant, null, null, true)]
        [InlineData("preamble:", "preamble", EmptyString, EmptyString, true)]
        [InlineData("preamble:\\", "preamble", EmptyString, EmptyString, true)]
        [InlineData("preamble:line 1", "preamble", "line 1", EmptyString, true)]
        [InlineData("preamble:line 1\\line 2", "preamble", "line 1", "line 2", true)]
        [InlineData("preamble:line 1\\line 2\\with a slash", "preamble", "line 1", "line 2\\with a slash", true)]
        [InlineData("preamble:line 1\nline 2", "preamble", "line 1", "nline 2", true)]
        [InlineData("preamble:line 1\bline 2", "preamble", "line 1", "bline 2", true)]
        [InlineData("preamble:line 1\"line 2", "preamble", "line 1", "\"line 2", true)]
        [InlineData("preamble:line 1\0line 2", "preamble", "line 1", "0line 2", true)]
        [InlineData("preamble:line 1\aline 2", "preamble", "line 1", "aline 2", true)]
        [InlineData("preamble:line 1\fline 2", "preamble", "line 1", "fline 2", true)]
        [InlineData("preamble:line 1\rline 2", "preamble", "line 1", "rline 2", true)]
        [InlineData("preamble:line 1\tline 2", "preamble", "line 1", "tline 2", true)]
        [InlineData("preamble:line 1\vline 2", "preamble", "line 1", "vline 2", true)]
        [InlineData("preamble:\\line 2", "preamble", EmptyString, "line 2", true)]
        [InlineData("preamble:\nline 2", "preamble", EmptyString, "nline 2", true)]
        [InlineData("preamble:\bline 2", "preamble", EmptyString, "bline 2", true)]
        [InlineData("preamble:that's the sound of an alarm\no going off", "preamble", "that's the sound of an alarm", "no going off", true)]
        public void PreambleTwoLineTest(string input, string expectedPreamble, string expectedTopLine, string expectedBottomLine, bool expectedResult)
        {
            var parser = new CommandParser();
            Command c = null;

            var result = parser.TryParse(input, out c);

            Assert.True(result == expectedResult);

            if (result)
            {
                Assert.NotNull(c);

                Assert.Equal(expectedPreamble, c.Preamble,StringComparer.OrdinalIgnoreCase);
                Assert.Equal(expectedTopLine, c.TopLine, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(expectedBottomLine, c.BottomLine, StringComparer.OrdinalIgnoreCase);
            }
            else
                Assert.Null(c);
        }
    }
}
