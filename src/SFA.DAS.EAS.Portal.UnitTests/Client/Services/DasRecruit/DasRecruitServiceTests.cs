using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Configuration;
using SFA.DAS.EAS.Portal.Client.Services.DasRecruit;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Client.Services.DasRecruit
{
    [TestFixture, Parallelizable]
    internal class DasRecruitServiceTests : FluentTest<DasRecruitServiceTestsFixture>
    {
        [Test, Ignore("work in progress")]
        public Task GetVacancies_X_y()
        {
            return TestAsync(f => f.GetVacancies(), (f, r) => r.Should());
        }
    }

    internal class DasRecruitServiceTestsFixture
    {
        public DasRecruitService DasRecruitService { get; set; }
        public RecruitApiClientConfiguration RecruitApiClientConfiguration { get; set; }
        public ILogger<DasRecruitService> Logger { get; set; }
        
        public DasRecruitServiceTestsFixture()
        {
            RecruitApiClientConfiguration = new RecruitApiClientConfiguration();
            Logger = new NullLogger<DasRecruitService>();

            DasRecruitService = new DasRecruitService(RecruitApiClientConfiguration, Logger);
        }

        public async Task<IEnumerable<Vacancy>> GetVacancies()
        {
            const long accountId = 123L;
            return await DasRecruitService.GetVacancies(accountId);
        }
    }
}