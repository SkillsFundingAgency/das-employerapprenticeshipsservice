using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EAS.Application.Extensions;
using SFA.DAS.EAS.Infrastructure.Extensions;

namespace SFA.DAS.EAS.Support.Infrastructure.Tests.Extensions
{
    [TestFixture]
    public class ExceptionExtensionTests
    {
        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithException_GetsExceptionHandler()
        {
            TestExpectedExceptionHandler(new Exception("outermessage"), typeof(Exception));
        }

        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithAggregateException_GetsAggregateExceptionHandler()
        {
            TestExpectedExceptionHandler(new AggregateException("outermessage"), typeof(AggregateException));
        }

        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithHttpException_GetsHttpExceptionHandler()
        {
            TestExpectedExceptionHandler(new HttpException("outermessage"), typeof(HttpException));
        }

        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithNonExplicitlySupportedException_GetsBestFitHandlerWhenDirectDescendent()
        {
            TestExpectedExceptionHandler(new ChildOfException(), typeof(Exception));
        }


        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithNonExplicitlySupportedException_GetsBestFitHandlerWhenAFewTypesDown()
        {
            TestExpectedExceptionHandler(new GrandChildOfException(), typeof(Exception));
        }

        [Test]
        public void GetAppropriateExceptionFormatter_WhenCalledWithNonExplicitlySupportedException_GetsBestFitHandlerWhenAFewMoreTypesDown()
        {
            TestExpectedExceptionHandler(new GreatGrandChildOfException(), typeof(Exception));
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithException_ContainsExceptionType()
        {
            TestDetailedMessage(new Exception("outer"), nameof(Exception));
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithException_ContainsExceptionMessage()
        {
            TestDetailedMessage(new Exception("outer"), "outer");
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithInnerException_ContainsOuterExceptionType()
        {
            TestDetailedMessage(new Exception("outer", new ApplicationException("inner")), nameof(Exception));
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithInnerException_ContainsOuterExceptionMessage()
        {
            TestDetailedMessage(new Exception("outer", new ApplicationException("inner")), "outer");
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithInnerException_ContainsInnerExceptionType()
        {
            TestDetailedMessage(new Exception("outer", new ApplicationException("inner")), nameof(ApplicationException));
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithInnerException_ContainsInnerExceptionMessage()
        {
            TestDetailedMessage(new Exception("outer", new ApplicationException("inner")), "inner");
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithAggregateException_ContainsAllExceptionessages()
        {
            TestDetailedMessage(new AggregateException(
                new Exception("aggregate1"),
                new ApplicationException("aggregate2"),
                new HttpException("aggregate3")),
                "aggregate1", "aggregate2", "aggregate3");
        }
        [Test]
        public void GetDetailedMessage_WhenCalledWithAggregateExceptionWithInnerExceptions_ContainsAllOuterExceptionessages()
        {
            TestDetailedMessage(new AggregateException(
                    new Exception("aggregate1", new Exception("inner1")),
                    new ApplicationException("aggregate2", new Exception("inner2")),
                    new HttpException("aggregate3", new Exception("inner3"))),
                "aggregate1", "aggregate2", "aggregate3");
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithAggregateExceptionWithInnerExceptions_ContainsAllInnerExceptionessages()
        {
            TestDetailedMessage(new AggregateException(
                    new Exception("aggregate1", new Exception("inner1")),
                    new ApplicationException("aggregate2", new Exception("inner2")),
                    new HttpException("aggregate3", new Exception("inner3"))),
                "inner1", "inner2", "inner3");
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithHttpExceptions_ContainsHttpErrorCode()
        {
            TestDetailedMessage(new HttpException((int)HttpStatusCode.Conflict, ""),
                ((int) HttpStatusCode.Conflict).ToString(CultureInfo.InvariantCulture));
        }

        [Test]
        public void GetDetailedMessage_WhenCalledWithHttpExceptions_ContainsHttpMessage()
        {
            const string webFailureMessage = "Sorry Dave, I can't let you do that.";
            TestDetailedMessage(new HttpException((int)HttpStatusCode.Conflict, webFailureMessage),
                webFailureMessage);
        }

        private void TestExpectedExceptionHandler(Exception exception, Type expectedExceptionType)
        {
            var actualHandler = ExceptionExtensions.GetAppropriateExceptionFormatter(exception);

            Assert.AreEqual(actualHandler.SupportedException, expectedExceptionType);
        }

        private void TestDetailedMessage(Exception exception, params string[] expectedMessageContents)
        {
            var detailedMessage = ExceptionExtensions.GetMessage(exception);

            foreach (var part in expectedMessageContents)
            {
                Assert.IsTrue(detailedMessage.Contains(part), $"detailed message {detailedMessage} does not contain expected part {part}");
            }
        }
    }

    class ChildOfException : Exception
    {

    }

    class GrandChildOfException : ChildOfException
    {

    }

    class GreatGrandChildOfException : GrandChildOfException
    {

    }
}
