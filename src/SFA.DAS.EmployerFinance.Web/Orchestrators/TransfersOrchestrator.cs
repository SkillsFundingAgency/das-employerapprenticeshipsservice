using System;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.HashingService;
using SFA.DAS.EmployerFinance.Web.ViewModels.Transfers;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Extensions;

namespace SFA.DAS.EmployerFinance.Web.Orchestrators
{
    public class TransfersOrchestrator
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IHashingService _hashingService;
        private readonly IManageApprenticeshipsService _manageApprenticeshipsService;
        private readonly IAccountApiClient _accountApiClient;        

        protected TransfersOrchestrator()
        {
        }

        public TransfersOrchestrator(
            IAuthorizationService authorizationService,
            IHashingService hashingService,
            IManageApprenticeshipsService manageApprenticeshipsService,
            IAccountApiClient accountApiClient)
        {
            _authorizationService = authorizationService;
            _hashingService = hashingService;
            _manageApprenticeshipsService = manageApprenticeshipsService;
            _accountApiClient = accountApiClient;            
        }

        public async Task<OrchestratorResponse<IndexViewModel>> GetIndexViewModel(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var indexTask = _manageApprenticeshipsService.GetIndex(accountId);
            var accountDetail = _accountApiClient.GetAccount(hashedAccountId);

            var renderCreateTransfersPledgeButtonTask = _authorizationService.IsAuthorizedAsync(EmployerUserRole.OwnerOrTransactor);            

            await Task.WhenAll(indexTask, renderCreateTransfersPledgeButtonTask, accountDetail);

            Enum.TryParse(accountDetail.Result.ApprenticeshipEmployerType, true, out ApprenticeshipEmployerType employerType);

            return new OrchestratorResponse<IndexViewModel>
            {
                Data = new IndexViewModel
                {
                    IsLevyEmployer = employerType == ApprenticeshipEmployerType.Levy,
                    PledgesCount = indexTask.Result.PledgesCount,
                    ApplicationsCount = indexTask.Result.ApplicationsCount,
                    RenderCreateTransfersPledgeButton = renderCreateTransfersPledgeButtonTask.Result,                    
                    StartingTransferAllowance = accountDetail.Result.StartingTransferAllowance,
                    FinancialYearString = DateTime.UtcNow.ToFinancialYearString(),
                    HashedAccountID = hashedAccountId
                }
            };
        }

        public async Task<OrchestratorResponse<FinancialBreakdownViewModel>> GetFinancialBreakdownViewModel(string hashedAccountId) 
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);
            var financialBreakdownTask = _manageApprenticeshipsService.GetFinancialBreakdown(accountId);
            var accountDetailTask = _accountApiClient.GetAccount(hashedAccountId);
            await Task.WhenAll(financialBreakdownTask, accountDetailTask);

            return new OrchestratorResponse<FinancialBreakdownViewModel>
            {
                Data = new FinancialBreakdownViewModel
                {
                    TransferConnections = financialBreakdownTask.Result.TransferConnections,
                    HashedAccountID = hashedAccountId,
                    AcceptedPledgeApplications = financialBreakdownTask.Result.AcceptedPledgeApplications + financialBreakdownTask.Result.PledgeOriginatedCommitments,
                    ApprovedPledgeApplications = financialBreakdownTask.Result.ApprovedPledgeApplications,
                    Commitments = financialBreakdownTask.Result.Commitments,
                    PledgeOriginatedCommitments = financialBreakdownTask.Result.PledgeOriginatedCommitments,                    
                    ProjectionStartDate = financialBreakdownTask.Result.ProjectionStartDate,
                    CurrentYearEstimatedSpend = financialBreakdownTask.Result.CurrentYearEstimatedCommittedSpend,
                    NextYearEstimatedSpend = financialBreakdownTask.Result.NextYearEstimatedCommittedSpend,
                    YearAfterNextYearEstimatedSpend = financialBreakdownTask.Result.YearAfterNextYearEstimatedCommittedSpend,
                    StartingTransferAllowance = accountDetailTask.Result.StartingTransferAllowance,
                    FinancialYearString = DateTime.UtcNow.ToFinancialYearString(),
                    NextFinancialYearString = DateTime.UtcNow.AddYears(1).ToFinancialYearString(),
                    YearAfterNextFinancialYearString = DateTime.UtcNow.AddYears(2).ToFinancialYearString(),
                    AmountPledged = financialBreakdownTask.Result.AmountPledged
                }
            };
        }
    }
}