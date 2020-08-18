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

namespace SFA.DAS.EmployerFinance.UnitTests.Services.ContentServiceTests
{
    class WhenIGetContentData
    {
        private Mock<IContentApiClient> MockContentApiClient;
        private IContentService ContentService;
        private string TestContent = "<h1My First Heading</h1><p>My First Paragraph</p>";

        [SetUp]
        public void SetUp()
        {
            MockContentApiClient = new Mock<IContentApiClient>();
            MockContentApiClient
                .Setup(c => c.Get("banner", "eas-fin"))
                .ReturnsAsync(TestContent);

            ContentService = new ContentService(MockContentApiClient.Object);
        }

        [Test]
        public async Task THEN_ContentIsReturnedFromApi()
        {
            var result = await ContentService.Get("banner", "eas-fin");

            result.Should().Be(TestContent);
        }
    }
}
