using NUnit.Framework;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.UnitTests.Factories.EmployerAgreementEventFactoryTests
{
    internal class WhenICreateAnEvent
    {
        private EmployerAgreementEventFactory _factory;
        private EmployerApprenticeshipApiConfiguration _apiConfiguration;

        [SetUp]
        public void Arrange()
        {
            _apiConfiguration = new EmployerApprenticeshipApiConfiguration
            {
                BaseUrl = "api/test/"
            };

            var configuration = new EmployerApprenticeshipsServiceConfiguration
            {
                EmployerApprenticeshipApi = _apiConfiguration
            };

            _factory = new EmployerAgreementEventFactory(configuration);
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
            Assert.AreEqual(_apiConfiguration.BaseUrl + expectedApiUrlPostfix, @event.ResourceUrl);
        }
    }
}
