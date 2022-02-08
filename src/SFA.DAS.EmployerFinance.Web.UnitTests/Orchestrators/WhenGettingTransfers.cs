﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.HashingService;
using SFA.DAS.OidcMiddleware.Builders;

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
            _featureTogglesService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(AccountId);
            _featureTogglesService.Setup(x => x.GetFeatureToggle(It.IsAny<string>())).Returns(new EmployerFeatureToggle { IsEnabled = true });

            _orchestrator = new TransfersOrchestrator(_authorisationService.Object, _hashingService.Object, _maService.Object, _accountApiClient.Object, _featureTogglesService.Object);
        }

        [Test]
        public async Task AndTheEmployerIsNonLevyThenCanViewApplySectionIsTrue()
        {
            _maService.Setup(o => o.GetIndex(AccountId)).ReturnsAsync(new GetIndexResponse
            {
                IsTransferSender = false
            });

            SetupTheAccountApiClient(false);

            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.IsTrue(actual.Data.CanViewApplySection);
        }

        [Test]
        public async Task AndTheEmployerIsLevyAndNotSendingFundsThenCanViewApplySectionIsTrue()
        {
            _maService.Setup(o => o.GetIndex(AccountId)).ReturnsAsync(new GetIndexResponse
            {
                IsTransferSender = false
            });

            SetupTheAccountApiClient(true);

            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.IsTrue(actual.Data.CanViewApplySection);
        }

        [Test]
        public async Task AndTheEmployerIsLevyAndSendingFundsThenCanViewApplySectionIsFalse()
        {
            _maService.Setup(o => o.GetIndex(AccountId)).ReturnsAsync(new GetIndexResponse
            {
                IsTransferSender = true
            });

            SetupTheAccountApiClient(true);

            var actual = await _orchestrator.GetIndexViewModel(HashedAccountId);

            Assert.IsFalse(actual.Data.CanViewApplySection);
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
