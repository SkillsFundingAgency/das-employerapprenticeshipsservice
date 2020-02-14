using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Web.Extensions;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Encoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ResourceNotFoundException = SFA.DAS.EmployerAccounts.Web.Exceptions.ResourceNotFoundException;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class Row1Panel1Orchestrator : UserVerificationOrchestratorBase
    {
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;     

        public Row1Panel1Orchestrator(ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService)
        {
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        //Needed for tests
        protected Row1Panel1Orchestrator()
        {
        }

        public virtual async Task<OrchestratorResponse<Row1Panel1ViewModel>> GetAccount(string hashedAccountId,long AccountId)
        {
            try
            {    
                //TO DO : Render View Test
                //var apprenticeshipResponse = (await _commitmentsApiClient.GetApprenticeships(new GetApprenticeshipsRequest { AccountId = AccountId, ProviderId = cohortsResponse?.FirstOrDefault().ProviderId })).Apprenticeships; */
                var apprenticeshipResponse = (await _commitmentsApiClient.GetApprenticeships(new GetApprenticeshipsRequest { AccountId = AccountId }))?.Apprenticeships;
                var cohortsResponse = (await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { AccountId = AccountId }))?.Cohorts;
                var draftApprenticeshipsResponse = (cohortsResponse != null) ? (await _commitmentsApiClient.GetDraftApprenticeships((long)cohortsResponse?.FirstOrDefault().CohortId))?.DraftApprenticeships : null;
                
                var viewModel = new Row1Panel1ViewModel
                {
                    CohortsCount = cohortsResponse?.Count(),
                    ApprenticeshipsCount = apprenticeshipResponse?.Count(),
                    NumberOfDraftApprentices = cohortsResponse?.FirstOrDefault().NumberOfDraftApprentices,
                    HasDraftApprenticeship = draftApprenticeshipsResponse?.Count == 1,
                    CourseName = draftApprenticeshipsResponse?.FirstOrDefault().CourseName, //CourseName
                    CourseStartDate = draftApprenticeshipsResponse?.FirstOrDefault().StartDate, //CourseStartDate
                    CourseEndDate = draftApprenticeshipsResponse?.FirstOrDefault().EndDate, //CourseEndDate                    
                    HashedDraftApprenticeshipId = (draftApprenticeshipsResponse != null) ? _encodingService.Encode((long)draftApprenticeshipsResponse?.FirstOrDefault().Id, EncodingType.ApprenticeshipId) : string.Empty,
                    ProviderName = cohortsResponse?.FirstOrDefault().ProviderName,  //Training Provider
                    CohortStatus = (cohortsResponse != null) ?  cohortsResponse.FirstOrDefault().GetStatus() : CohortStatus.Unknown, //Status
                    HashedCohortReference = (cohortsResponse != null) ? _encodingService.Encode((long)cohortsResponse?.FirstOrDefault().CohortId, EncodingType.CohortReference) : string.Empty,
                    ApprenticeName = draftApprenticeshipsResponse?.FirstOrDefault().FirstName + " " + draftApprenticeshipsResponse?.FirstOrDefault().LastName
                };

                return new OrchestratorResponse<Row1Panel1ViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = viewModel
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new OrchestratorResponse<Row1Panel1ViewModel>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = ex
                };
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                return new OrchestratorResponse<Row1Panel1ViewModel>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = new ResourceNotFoundException($"An error occured whilst trying to retrieve account: {hashedAccountId}", ex)
                };
            }
            catch (Exception ex)
            {
                return new OrchestratorResponse<Row1Panel1ViewModel>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = ex
                };
            }
        }
    }
}