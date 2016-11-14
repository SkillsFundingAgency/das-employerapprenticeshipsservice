using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.PaymentProvider.Worker.Providers;

namespace SFA.DAS.EAS.PaymentProvider.Worker.UnitTests.Providers.PaymentDataProcessorTests
{
    public class WhenIProcessPaymentData
    {
        private PaymentDataProcessor _paymentDataProcessor;

        [SetUp]
        public void Arrange()
        {
            _paymentDataProcessor = new PaymentDataProcessor();
        }
    }
}
