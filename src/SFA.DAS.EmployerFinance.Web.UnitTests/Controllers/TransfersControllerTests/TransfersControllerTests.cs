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
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels.Transfers;
using SFA.DAS.HashingService;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    public class TransfersControllerTests
    {
        private TransfersController _controller;
        private Mock<TransfersOrchestrator> _orchestrator;
        private Mock<IHashingService> _hashingService;
        private Mock<IAuthorizationService> _authorisationService;
        private Mock<IManageApprenticeshipsService> _maService;
        private Mock<IAccountApiClient> _accountApiClient;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureTogglesService;
        private GetFinancialBreakdownResponse _financialBreakdownResponse;
        private AccountDetailViewModel _accountDetailViewModel;

        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;

        [SetUp]
        public void Arrange()
        {
            var fixture = new Fixture();

            _authorisationService = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _maService = new Mock<IManageApprenticeshipsService>();
            _accountApiClient = new Mock<IAccountApiClient>();
            _featureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();
            _financialBreakdownResponse = fixture.Create<GetFinancialBreakdownResponse>();

            _maService.Setup(m => m.GetFinancialBreakdown(AccountId)).ReturnsAsync(_financialBreakdownResponse);

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);
            _featureTogglesService.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new EmployerFeatureToggle { IsEnabled = true });
            _accountDetailViewModel = fixture.Create<AccountDetailViewModel>();
            _accountApiClient.Setup(m => m.GetAccount(HashedAccountId)).ReturnsAsync(_accountDetailViewModel);
            _orchestrator = new Mock<TransfersOrchestrator>(_authorisationService.Object, _hashingService.Object, _maService.Object, _accountApiClient.Object);

            _controller = new TransfersController(_orchestrator.Object);
        }

        [Test]
        public async Task FinancialBreakdownReturnsAViewModelWithData()
        {   
            var result = await _controller.FinancialBreakdown(HashedAccountId);
            var view = result as ViewResult;
            var viewModel = view?.Model as OrchestratorResponse<FinancialBreakdownViewModel>;
           
            Assert.IsNotNull(viewModel.Data);
            Assert.IsNotNull(viewModel.Data.AcceptedPledgeApplications);
            Assert.IsNotNull(viewModel.Data.ApprovedPledgeApplications);
            Assert.IsNotNull(viewModel.Data.Commitments);
            Assert.IsNotNull(viewModel.Data.TransferConnections);
            Assert.IsNotNull(viewModel.Data.CurrentYearEstimatedSpend);
            Assert.IsNotNull(viewModel.Data.NextYearEstimatedSpend);
            Assert.IsNotNull(viewModel.Data.YearAfterNextYearEstimatedSpend);
        }        

        [Test]
        public async Task FinancialBreakdownPageShowsEstimatedRemainingAllowance()
        {
            var viewModel = await GetViewModel();

            var estimatedRemainingAllowance = viewModel.Data.StartingTransferAllowance - viewModel.Data.CurrentYearEstimatedSpend;
            Assert.AreEqual(estimatedRemainingAllowance, viewModel.Data.EstimatedRemainingAllowance);
        }

        [Test]
        public async Task FinancialBreakdownPageShowsCorrectTotalAvailablePledgedFunds()
        {
            var viewModel = await GetViewModel();

            var totalAvailablePledgedFunds = viewModel.Data.TotalAvailableTransferAllowance - viewModel.Data.TotalPledgedAndTransferConnections;
            Assert.AreEqual(totalAvailablePledgedFunds, viewModel.Data.TotalAvailablePledgedFunds);
        }

        [Test]
        public async Task FinancialBreakdownPageShowsCorrectTotalPledgedAndTransferConnections()
        {
            var viewModel = await GetViewModel();

            var totalPledgedAndTransferConnections = viewModel.Data.AmountPledged + viewModel.Data.TransferConnections;
            Assert.AreEqual(totalPledgedAndTransferConnections, viewModel.Data.TotalPledgedAndTransferConnections);
        }

        [Test]
        public async Task FinancialBreakdownPageShowsCorrectAvailablePledgedFunds()
        {
            var viewModel = await GetViewModel();

            var availablePledgedFunds = viewModel.Data.AmountPledged - (viewModel.Data.ApprovedPledgeApplications + viewModel.Data.AcceptedPledgeApplications);
            Assert.AreEqual(availablePledgedFunds, viewModel.Data.AvailablePledgedFunds);
        }

        private async Task<OrchestratorResponse<FinancialBreakdownViewModel>> GetViewModel()
        {
            var result = await _controller.FinancialBreakdown(HashedAccountId);

            var view = result as ViewResult;
            var viewModel = view?.Model as OrchestratorResponse<FinancialBreakdownViewModel>;

            Assert.IsNotNull(viewModel);

            return viewModel;
        }
    }
}
