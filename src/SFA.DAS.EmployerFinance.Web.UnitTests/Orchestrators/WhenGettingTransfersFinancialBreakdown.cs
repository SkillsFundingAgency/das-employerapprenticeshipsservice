using AutoFixture;
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
        private Mock<ITransfersService> _transfersService;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureTogglesService;
        private GetFinancialBreakdownResponse _financialBreakdownResponse;

        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();

            _authorisationService = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _transfersService = new Mock<ITransfersService>();
            _accountApiClient = new Mock<IAccountApiClient>();
            _featureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();
            _financialBreakdownResponse = fixture.Create<GetFinancialBreakdownResponse>();

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);
            _featureTogglesService.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new EmployerFeatureToggle { IsEnabled = true });

            _accountApiClient.Setup(m => m.GetAccount(HashedAccountId)).ReturnsAsync(new AccountDetailViewModel
            {
                AccountId = AccountId
            });

            _orchestrator = new TransfersOrchestrator(_authorisationService.Object, _hashingService.Object, _transfersService.Object, _accountApiClient.Object);
        }
        [Test]
        public async Task CheckFinancialBreakdownViewModel()
        {
            _transfersService.Setup(o => o.GetFinancialBreakdown(AccountId)).ReturnsAsync(_financialBreakdownResponse);

            var actual = await _orchestrator.GetFinancialBreakdownViewModel(HashedAccountId);

            Assert.AreEqual(_financialBreakdownResponse.AcceptedPledgeApplications + _financialBreakdownResponse.PledgeOriginatedCommitments, actual.Data.AcceptedPledgeApplications);
            Assert.AreEqual(_financialBreakdownResponse.ApprovedPledgeApplications, actual.Data.ApprovedPledgeApplications);
            Assert.AreEqual(_financialBreakdownResponse.Commitments, actual.Data.Commitments);
            Assert.AreEqual(_financialBreakdownResponse.TransferConnections, actual.Data.TransferConnections);
            Assert.AreEqual(_financialBreakdownResponse.PledgeOriginatedCommitments, actual.Data.PledgeOriginatedCommitments);
        }
    }
}
