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
        [InlineData("preamble:", "preamble", EmptyString, EmptyString, true)]
        [InlineData("preamble:line 1", "preamble", "line 1", EmptyString, true)]
        [InlineData("preamble:line 1\\line 2", "preamble", "line 1", "line 2", true)]
        [InlineData("preamble:line 1\nline 2", "preamble", "line 1", "nline 2", true)]
        [InlineData("preamble:line 1\bline 2", "preamble", "line 1", "bline 2", true)]
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

                Assert.Equal(c.Preamble, expectedPreamble, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(c.TopLine, expectedTopLine, StringComparer.OrdinalIgnoreCase);
                Assert.Equal(c.BottomLine, expectedBottomLine, StringComparer.OrdinalIgnoreCase);
            }
            else
                Assert.Null(c);
        }
    }
}
