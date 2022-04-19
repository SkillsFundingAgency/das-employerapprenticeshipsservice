using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.HashingService;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class WhenGettingTransfersFinancialBreakdown
    {
        private TransfersOrchestrator _orchestrator;
        private Mock<IAuthorizationService> _authorisationService;
        private Mock<IHashingService> _hashingService;
        private Mock<IManageApprenticeshipsService> _maService;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureTogglesService;

        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;

        [SetUp]
        public void Setup()
        {
            _authorisationService = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _maService = new Mock<IManageApprenticeshipsService>();
            _accountApiClient = new Mock<IAccountApiClient>();
            _featureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);
            _featureTogglesService.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new EmployerFeatureToggle { IsEnabled = true });

            _orchestrator = new TransfersOrchestrator(_authorisationService.Object, _hashingService.Object, _maService.Object, _accountApiClient.Object, _featureTogglesService.Object);
        }

        [Test]
        public async Task CheckFinancialBreakdownViewModel()
        {
            var financialBreakdownResponse = new GetFinancialBreakdownResponse
            {
                AcceptedPledgeApplications = 20000,
                ApprovedPledgeApplications = 10000,
                Commitments = 1000,
                TransferConnections = 1000,
                NumberOfMonths = 12
            };

            _maService.Setup(o => o.GetFinancialBreakdown(AccountId)).ReturnsAsync(financialBreakdownResponse);

            SetupTheAccountApiClient();

            var actual = await _orchestrator.GetFinancialBreakdownViewModel(HashedAccountId);

            Assert.AreEqual(financialBreakdownResponse.AcceptedPledgeApplications, actual.Data.AcceptedPledgeApplications);
            Assert.AreEqual(financialBreakdownResponse.ApprovedPledgeApplications, actual.Data.ApprovedPledgeApplications);
            Assert.AreEqual(financialBreakdownResponse.Commitments, actual.Data.Commitments);
            Assert.AreEqual(financialBreakdownResponse.TransferConnections, actual.Data.TransferConnections);
            Assert.AreEqual(financialBreakdownResponse.NumberOfMonths, actual.Data.NumberOfMonths);
        }

        private void SetupTheAccountApiClient()
        {
            var modelToReturn = new AccountDetailViewModel
            {
                ApprenticeshipEmployerType = "Levy"
            };

            _accountApiClient.Setup(o => o.GetAccount(HashedAccountId)).ReturnsAsync(modelToReturn);
        }


    }
}
