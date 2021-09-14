using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.CohortsServiceTests
{
    public class WhenIGetACohortsCount
    {
        private Mock<IApiClient> _apiClient;
        private ApprenticeshipService _service;

        [SetUp]
        public void Setup()
        {
            _apiClient = new Mock<IApiClient>();
            _service = new ApprenticeshipService(_apiClient.Object);
        }

        [Test]
        public async Task ThenTheApiIsCalledWithAValidAccountIdAndTheCohortsCountIsReturned()
        {
            SetupApiClient(1);

            var actual = await _service.GetApprenticeshipsFor(1);

            Assert.AreEqual(1, actual);
        }

        [Test]
        public async Task ThenTheApiIsCalledWithAnInvalidAccountIdAndTheCohortsCountIsZero()
        {
            SetupApiClient(0, false);

            var actual = await _service.GetApprenticeshipsFor(0);

            Assert.AreEqual(0, actual);
        }

        private void SetupApiClient(long accountId, bool addItem = true)
        {
            var items = new List<CohortSummary>();

            if (addItem)
            {
                items = new List<CohortSummary>
                {
                    new CohortSummary
                    {
                        AccountId = accountId
                    }
                };
            }
            
            _apiClient.Setup(o =>
                    o.Get<GetApplicationsResponse>(
                        It.Is<GetApplicationsRequest>(i => i.GetUrl.Equals($"api/cohorts?accountId={accountId}"))))
                .ReturnsAsync(new GetApplicationsResponse
                {
                    //Apprenticeships = items
                });
        }
    }
}
