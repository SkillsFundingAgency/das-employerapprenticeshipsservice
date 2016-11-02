using System.Threading.Tasks;
using NUnit.Framework;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.UnitTests.UpdaterTests
{
    public class WhenIUpdatePayments
    {
        private Updater.PaymentProcessor _paymentProcessor;

        [SetUp]
        public void Arrange()
        {
            _paymentProcessor = new Updater.PaymentProcessor();
        }

        [Test]
        public async Task ThenThePaymentsApiShouldBeCheckedForTheCurrentPeriodEnd()
        {
            //Act
            await _paymentProcessor.RunUpdate();

            //Assert

        }

        [Test]
        public async Task ThenTheCurrentPeriodEndShouldBeStoredIfDifferent()
        {
            
        }

        [Test]
        public async Task ThenTheAccountMessagesShouldBeAddedToTheQueueIfThePeriodEndHasChangedI()
        {
            
        }
    }
}
