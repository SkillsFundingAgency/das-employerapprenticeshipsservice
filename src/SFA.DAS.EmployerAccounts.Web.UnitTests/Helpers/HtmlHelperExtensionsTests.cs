using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Web.Helpers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    {
        private string _expectedOutput;
        private string _expectedLabelOne;
        private string _expectedLabelTwo;
        private string _actualOutput;

        [SetUp]
        public void WhenICallSetZenDeskLabelsWithMultipleLabelsWithMultipleWordsAndApostrophes()
        {
            _expectedLabelOne = "a string with multiple words";
            _expectedLabelTwo = "the title of another page";
            _expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: ['{_expectedLabelOne}','{_expectedLabelTwo}'] }});</script>";
            _actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, new string[] {_expectedLabelOne, _expectedLabelTwo}).ToString();
        }

        [Test]
        public void ThenTheOutputIsCorrect()
        {
            Assert.That(_actualOutput, Is.EqualTo(_expectedOutput));
        }
    }
}
