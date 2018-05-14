using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.EAS.Domain.Models.Payments;
using SFA.DAS.EAS.Infrastructure.Extensions;
using System;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Extensions
{
    [TestFixture]
    public class PaymentDetailsExtensionsTests
    {


        [TestCase("123456", "123456", "123456", "123456", false)]
        [TestCase("123456789012345678901", "123456", "123456", "123456", true)]
        [TestCase("123456", "1234567890123456789012345678901234567890123456789012345678901", "123456", "123456", true)]
        [TestCase("123456", "123456", "1234567890123456789012345X", "123456", true)]
        [TestCase("123456", "123456", "123456", "1234567890123456789012345X", true)]
        public void Check(string collectionPeriodId, string employerAccountVersion, string apprenticeshipVersion, string periodEnd, bool expectExcpetion)
        {
            var paymentDetail = new PaymentDetails
            {
                CollectionPeriodId = collectionPeriodId,
                EmployerAccountVersion = employerAccountVersion,
                ApprenticeshipVersion = apprenticeshipVersion,
                PeriodEnd = periodEnd
            };

            if (expectExcpetion)
            {
                Assert.Throws<Exception>(paymentDetail.AssertValidPayment);
            }
            else
            {
                paymentDetail.AssertValidPayment();
                Assert.Pass("did not throw exception");
            }
        }
    }
}
