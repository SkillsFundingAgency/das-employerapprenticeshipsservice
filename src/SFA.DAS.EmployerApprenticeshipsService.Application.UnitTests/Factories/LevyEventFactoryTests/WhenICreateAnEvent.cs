using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Factories;

namespace SFA.DAS.EAS.Application.UnitTests.Factories.LevyEventFactoryTests
{
    internal class WhenICreateAnEvent
    {
        private LevyEventFactory _factory;
        
        [SetUp]
        public void Arrange()
        {
            _factory = new LevyEventFactory();
        }

        [Test]
        public void ThenIShouldGetTheCorrectResourceUrl()
        {
            //Arrange
            const string hashedAccountId = "HJKH23423";
            const string payrollYear = "17-18";
            const short payrollMonth = 3;
            
            var expectedApiUrl = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}";

            //Act
            var @event = _factory.CreateDeclarationUpdatedEvent(hashedAccountId, payrollYear, payrollMonth);

            //Assert
            @event.ResourceUri.Should().Be(expectedApiUrl);
        }
    }
}
