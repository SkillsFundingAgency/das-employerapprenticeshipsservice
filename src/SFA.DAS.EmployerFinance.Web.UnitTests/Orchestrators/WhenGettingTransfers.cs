﻿using System.Threading.Tasks;
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
    public class WhenGettingTransfers
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

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);            

            _orchestrator = new TransfersOrchestrator(_authorisationService.Object, _hashingService.Object, _maService.Object, _accountApiClient.Object);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task Only_Levy_Payer_Can_View_Pledges_Section(bool isLevyPayer, bool expectCanViewPledgesSection)
        {
            _maService.Setup(o => o.GetIndex(AccountId)).ReturnsAsync(new GetIndexResponse());

            SetupTheAccountApiClient(isLevyPayer);
            
            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.AreEqual(expectCanViewPledgesSection, actual.Data.CanViewPledgesSection);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public async Task ThenChecksTheUserIsAuthorisedToCreateTransfers(bool isAuthorised, bool expected)
        {
            _maService.Setup(o => o.GetIndex(AccountId)).ReturnsAsync(new GetIndexResponse());

            SetupTheAccountApiClient(true);

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
