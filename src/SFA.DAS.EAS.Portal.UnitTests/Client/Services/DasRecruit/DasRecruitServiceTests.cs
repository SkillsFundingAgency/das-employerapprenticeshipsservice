using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Http;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.UnitTests.Fakes;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Client.Services.DasRecruit
{
    [TestFixture, Parallelizable]
    internal class DasRecruitServiceTests : FluentTest<DasRecruitServiceTestsFixture>
    {
        [Test, Ignore("wip")]
        public Task GetVacancies_X_y()
        {
            return TestAsync(f => f.GetVacancies(), (f, r) => r.Should());
        }
    }

    internal class DasRecruitServiceTestsFixture
    {
        public DasRecruitService DasRecruitService { get; set; }
        public ILogger<DasRecruitService> Logger { get; set; }
        public Mock<RecruitApiHttpClientFactory> RecruitApiHttpClientFactory { get; set; }
        public FakeHttpMessageHandler HttpMessageHandler { get; set; }
        public HttpClient HttpClient { get; set; }
        
        public DasRecruitServiceTestsFixture()
        {
            Logger = new NullLogger<DasRecruitService>();

            HttpMessageHandler = new FakeHttpMessageHandler();
            HttpClient = new HttpClient(HttpMessageHandler) { BaseAddress = new Uri("https://example.com") };

            RecruitApiHttpClientFactory = new Mock<RecruitApiHttpClientFactory>();
            RecruitApiHttpClientFactory.Setup(f => f.CreateHttpClient()).Returns(HttpClient);
            
            DasRecruitService = new DasRecruitService(RecruitApiHttpClientFactory.Object, Logger);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies()
        {
            const long accountId = 123L;
            return await DasRecruitService.GetVacancies(accountId);
        }
    }
}