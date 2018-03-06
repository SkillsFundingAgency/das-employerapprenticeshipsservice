using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor;

namespace SFA.DAS.EAS.DBMaintenance.WebJob.UnitTests.IdProcessor
{
    [TestFixture]
    public class ProcessingContextTests
    {
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(1234567890)]
        public void SetGet_WithAnInt_ReturnsExpectedValue(int i)
        {
            CheckRetrievalOfSavedValue(i);
        }

        [TestCase("a value")]
        [TestCase("")]
        [TestCase(null)]
        public void SetGet_WithString_ReturnsExpectedValue(string s)
        {
            CheckRetrievalOfSavedValue(s);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void SetGet_WithBool_ReturnsExpectedValue(bool b)
        {
            CheckRetrievalOfSavedValue(b);
        }

        [Test]
        public void SetGet_WithDate_ReturnsExpectedValue()
        {
            CheckRetrievalOfSavedValue(new DateTime(2018, 03, 15, 12, 30, 0));
        }

        private void CheckRetrievalOfSavedValue<T>(T setValue)
        {
            var processingContext=  new ProcessingContext();

            processingContext.Set("value", setValue);

            T actualValue = processingContext.Get<T>("value");

            Assert.AreEqual(setValue, actualValue);
        }
    }
}
