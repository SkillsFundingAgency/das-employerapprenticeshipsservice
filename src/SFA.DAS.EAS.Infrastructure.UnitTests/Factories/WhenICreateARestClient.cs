using System;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Factories;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Factories
{
    internal class WhenICreateARestClient
    {
        private RestClientFactory _factory;

        [SetUp]
        public void Arrange()
        {
            _factory = new RestClientFactory();
        }

        [Test]
        public void ThenItShouldHaveTheCorrectBaseUrl()
        {
            //Arange
            var baseUrl = new Uri("http://www.google.co.uk");

            //Act
            var client = _factory.Create(baseUrl);

            //Assert
            Assert.AreEqual(baseUrl, client.BaseUrl);
        }
    }
}
