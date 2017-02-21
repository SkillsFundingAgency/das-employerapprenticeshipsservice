using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;

using Moq;

using NLog;

using NUnit.Framework;

using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.ProviderEmailLookupService
{
    public class ProviderEmailLookupServiceTests
    {
        private Infrastructure.Services.ProviderEmailLookupService _sut;

        private readonly EmployerApprenticeshipsServiceConfiguration _config;
        private Mock<IdamsEmailServiceWrapper> _mockIdamsService;
        private Mock<IApprenticeshipInfoServiceWrapper> _mockApprenticeshipService;

        public ProviderEmailLookupServiceTests()
        {
            _config = new EmployerApprenticeshipsServiceConfiguration
                         {
                             CommitmentNotification =
                                 new CommitmentNotificationConfiguration
                                     {
                                         UseProviderEmail = false,
                                         ProviderTestEmails = new List<string> { "test@email.uk", "test2@email.uk" },
                                         IdamsListUsersUrl = "http://www.url.uk/path/to?query={0}"
                                     }
                         };
        }

        [SetUp]
        public void SetUp()
        {
            _mockIdamsService = new Mock<IdamsEmailServiceWrapper>(Mock.Of<ILogger>(), _config);
            _mockIdamsService.Setup(m => m.GetEmailsAsync(It.IsAny<long>())).ReturnsAsync(new List<string>());

            _mockApprenticeshipService = new Mock<IApprenticeshipInfoServiceWrapper>();
            _sut = new Infrastructure.Services.ProviderEmailLookupService(
                Mock.Of<ILogger>(),
                _mockIdamsService.Object,
                _config,
                _mockApprenticeshipService.Object
                );
        }

        [Test]
        public async Task WhenNoAddressesAreFound()
        {
            _config.CommitmentNotification.UseProviderEmail = true;

            var result = await _sut.GetEmailsAsync(123456L);
            result.Count.Should().Be(0);
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task WhenUsingATestEmail()
        {
            _config.CommitmentNotification.UseProviderEmail = false;

            var result = await _sut.GetEmailsAsync(123456L);
            result.Count.Should().Be(2);
            result[0].Should().Be("test@email.uk");
            result[1].Should().Be("test2@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Never);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task WhenAddressInProviderApi()
        {
            _config.CommitmentNotification.UseProviderEmail = true;
            _mockApprenticeshipService.Setup(m => m.GetProvider(It.IsAny<int>()))
                .Returns(
                    new ProvidersView
                        {
                            CreatedDate = DateTime.Now,
                            Providers = new List<Provider> { new Provider { Email = "provider@email.uk" } }
                            
                        });
            var result = await _sut.GetEmailsAsync(123456L);
            result.Count.Should().Be(1);
            result[0].Should().Be("provider@email.uk");
            _mockIdamsService.Verify(m => m.GetEmailsAsync(It.IsAny<long>()), Times.Once);
            _mockApprenticeshipService.Verify(m => m.GetProvider(It.IsAny<int>()), Times.Once);
        }
    }
}
