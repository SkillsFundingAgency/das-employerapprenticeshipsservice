using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Transfers;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Orchestrators
{
    [TestFixture]
    public class WhenGettingTransfersCounts
    {
        private TransfersOrchestrator _orchestrator;
        private Mock<IAuthorizationService> _authorisationService;
        private Mock<IHashingService> _hashingService;
        private Mock<ITransfersService> _transfersService;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureTogglesService;

        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;

        [SetUp]
        public void Setup()
        {
            _authorisationService = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _transfersService = new Mock<ITransfersService>();
            _accountApiClient = new Mock<IAccountApiClient>();

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);

            _orchestrator = new TransfersOrchestrator(_authorisationService.Object, _hashingService.Object, _transfersService.Object, _accountApiClient.Object);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task Only_Levy_Payer_Can_View_Pledges_Section(bool isLevyPayer, bool expectIsLevyEmployer)
        {
            _transfersService.Setup(o => o.GetCounts(AccountId)).ReturnsAsync(new GetCountsResponse());

            SetupTheAccountApiClient(isLevyPayer);

            _transfersService.Setup(o => o.GetFinancialBreakdown(AccountId)).ReturnsAsync(new GetFinancialBreakdownResponse());

            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.AreEqual(expectIsLevyEmployer, actual.Data.IsLevyEmployer);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ThenChecksTheUserIsAuthorisedToCreateTransfers(bool isAuthorised, bool expected)
        {
            _transfersService.Setup(o => o.GetCounts(AccountId)).ReturnsAsync(new GetCountsResponse());

            SetupTheAccountApiClient(true);

            _transfersService.Setup(o => o.GetFinancialBreakdown(AccountId)).ReturnsAsync(new GetFinancialBreakdownResponse());

            _authorisationService.Setup(o => o.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor)).ReturnsAsync(isAuthorised);

            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.AreEqual(expected, actual.Data.RenderCreateTransfersPledgeButton);
        }

        private void SetupTheAccountApiClient(bool isLevy = false)
        {
            var modelToReturn = new AccountDetailViewModel
            {
                ApprenticeshipEmployerType = isLevy ? "Levy" : "NonLevy"
            };

            _accountApiClient.Setup(o => o.GetAccount(HashedAccountId)).ReturnsAsync(modelToReturn);
        }
    }
}
