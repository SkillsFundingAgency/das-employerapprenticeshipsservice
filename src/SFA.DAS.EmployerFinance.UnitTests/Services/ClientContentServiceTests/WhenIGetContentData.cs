using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ClientContentServiceTests
{
    class WhenIGetContentData
    {
        private Mock<IClientContentApiClient> MockClientContentApiClient;
        private IClientContentService ClientContentService;
        private string TestContent = "<h1My First Heading</h1><p>My First Paragraph</p>";

        [SetUp]
        public void SetUp()
        {
            MockClientContentApiClient = new Mock<IClientContentApiClient>();
            MockClientContentApiClient
                .Setup(c => c.Get("banner", "eas-fin"))
                .ReturnsAsync(TestContent);

            ClientContentService = new ClientContentService(MockClientContentApiClient.Object);
        }

        public async Task THEN_Content_is_returned_from_api()
        {
            var result = await ClientContentService.Get("banner", "eas-fin");

            result.Should().Be(TestContent);
        }
    }
}
