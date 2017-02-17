using System.Collections.Generic;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;

using SFA.DAS.EAS.Application.Queries.GetProviderEmailQuery;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetProviderEmailQuery
{
    [TestFixture]
    public class WhenGetProviderEmailQuery
    {
        [Test]
        public async Task NotSendToProviderWithMockEmails()
        {
            var config = new EmployerApprenticeshipsServiceConfiguration
                             {
                                 ProviderTestEmails = new List<string> { "test1@email.com", "test2@email.com" },
                                 SendEmailToProviders = false
                             };

            var _sut = new GetProviderEmailQueryHandler(config, Mock.Of<ILogger>());
            
            var response = await _sut.Handle(new GetProviderEmailQueryRequest { ProviderId = 5L });

            response.Emails.Count.ShouldBeEquivalentTo(2);
            response.Emails[0].Should().Be("test1@email.com");
            response.Emails[1].Should().Be("test2@email.com");
        }

        [Test]
        public async Task NotSendToProviderWithMissingMockEmails()
        {
            var config = new EmployerApprenticeshipsServiceConfiguration
            {
                ProviderTestEmails = new List<string>(),
                SendEmailToProviders = false
            };

            var _sut = new GetProviderEmailQueryHandler(config, Mock.Of<ILogger>());

            var response = await _sut.Handle(new GetProviderEmailQueryRequest { ProviderId = 5L });

            response.Emails.Count.ShouldBeEquivalentTo(0);
        }

        [Test]
        public async Task SendToProvider()
        {
            var config = new EmployerApprenticeshipsServiceConfiguration
            {
                ProviderTestEmails = new List<string>(),
                SendEmailToProviders = false
            };

            var _sut = new GetProviderEmailQueryHandler(config, Mock.Of<ILogger>());

            var response = await _sut.Handle(new GetProviderEmailQueryRequest { ProviderId = 5L });

            response.Emails.Count.ShouldBeEquivalentTo(0);
        }

    }
}
