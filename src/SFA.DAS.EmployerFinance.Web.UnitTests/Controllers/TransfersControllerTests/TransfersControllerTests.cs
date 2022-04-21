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
using System;
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

        private const string HashedAccountId = "123ABC";
        private const long AccountId = 1234;

        [SetUp]
        public void Arrange()
        {
            _authorisationService = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _maService = new Mock<IManageApprenticeshipsService>();
            _accountApiClient = new Mock<IAccountApiClient>();
            _featureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _maService.Setup(m => m.GetFinancialBreakdown(AccountId)).ReturnsAsync(new GetFinancialBreakdownResponse
                {  
                    AcceptedPledgeApplications = 2000,
                    ApprovedPledgeApplications = 2000,
                    Commitments = 2000,
                    NumberOfMonths = 12,
                    PledgeOriginatedCommitments = 2000,
                    ProjectionStartDate= DateTime.Now,
                    FundsIn = 100000,
                    TransferConnections = 1000
                });

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);
            _featureTogglesService.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new EmployerFeatureToggle { IsEnabled = true });

            _accountApiClient.Setup(m => m.GetAccount(HashedAccountId)).ReturnsAsync(new AccountDetailViewModel
            {
                AccountId = AccountId,
                StartingTransferAllowance = 20000
            });

            _orchestrator = new Mock<TransfersOrchestrator>(_authorisationService.Object, _hashingService.Object, _maService.Object, _accountApiClient.Object, _featureTogglesService.Object);

            _controller = new TransfersController(_orchestrator.Object);
        }

        [Test]
        public async Task FinancialBreakdownReturnsAViewModel()
        {
            var result = await _controller.FinancialBreakdown(HashedAccountId);
                       
            //Assert
            var view = result as ViewResult;

            var viewModel = view?.Model as OrchestratorResponse<FinancialBreakdownViewModel>;
            Assert.IsNotNull(viewModel);
            Assert.AreEqual(2000, viewModel.Data.AcceptedPledgeApplications);
        }

        [Test]
        public async Task FinancialBreakdownPageShowsEstimatedRemainingAllowance()
        {
            var result = await _controller.FinancialBreakdown(HashedAccountId);

            //Assert
            var view = result as ViewResult;

            var viewModel = view?.Model as OrchestratorResponse<FinancialBreakdownViewModel>;
            Assert.IsNotNull(viewModel);
            var estimatedRemainingAllowance = viewModel.Data.TotalAvailableTransferAllowance - viewModel.Data.TotalEstimatedSpend;
            Assert.AreEqual(estimatedRemainingAllowance, viewModel.Data.EstimatedRemainingAllowance);
        }
    }
}
