using NUnit.Framework;
using SFA.DAS.EAS.Application.Factories;

namespace SFA.DAS.EAS.Application.UnitTests.Factories.EmployerAgreementEventFactoryTests
{
    internal class WhenICreateAnEvent
    {
        private EmployerAgreementEventFactory _factory;
        
        [SetUp]
        public void Arrange()
        {

            _factory = new EmployerAgreementEventFactory();
        }

        [Test]
        public void ThenIShouldGetTheCorrectResourceUrl()
        {
            //Arrange
            const string hashedAccountId = "HJKH23423";
            const string hashedLegalEntityId = "FDF654";
            const string hashedAgreementId = "3GF3KH";
            
            var expectedApiUrlPostfix =
                $"api/accounts/{hashedAccountId}/legalEntities/{hashedLegalEntityId}/agreements/{hashedAgreementId}";

            //Act
            var @event = _factory.CreateSignedEvent(hashedAccountId, hashedLegalEntityId, hashedAgreementId);

            //Assert
            Assert.AreEqual(expectedApiUrlPostfix, @event.ResourceUrl);
        }

        [Test]
        public void ThenIGetTheCorrectParamtersSetWhenRemovingTheAgreement()
        {
            //Arrange
            const string hashedAgreementId = "3GF3KH";

            //Act
            var actual = _factory.RemoveAgreementEvent(hashedAgreementId);

            //Assert
            Assert.AreEqual(hashedAgreementId, actual.HashedAgreementId);

        }
    }
}
