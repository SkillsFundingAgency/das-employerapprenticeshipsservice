using MediatR;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Requests;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.CommitmentsV2.Types.Dtos;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
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
        private readonly IMediator _mediator;
        private readonly ICommitmentsApiClient _commitmentsApiClient;
        private readonly IEncodingService _encodingService;     

        public Row1Panel1Orchestrator(IMediator mediator, ICommitmentsApiClient commitmentsApiClient, IEncodingService encodingService) :  base(mediator)
        {
            _mediator = mediator;
            _commitmentsApiClient = commitmentsApiClient;
            _encodingService = encodingService;
        }

        //Needed for tests
        protected Row1Panel1Orchestrator()
        {
        }

        public virtual async Task<OrchestratorResponse<CallToActionViewModel>> GetAccount(string hashedAccountId,long AccountId, string externalUserId)
        {
            try
            {
                var reservationsResponseTask =  _mediator.SendAsync(new GetReservationsRequest
                {
                    HashedAccountId = hashedAccountId,
                    ExternalUserId = externalUserId
                });


                var agreementsResponseTask =  _mediator.SendAsync(new GetAccountEmployerAgreementsRequest
                {
                    HashedAccountId = hashedAccountId,
                    ExternalUserId = externalUserId
                });

                await Task.WhenAll(reservationsResponseTask, agreementsResponseTask);

                var reservationsResponse = reservationsResponseTask.Result;
                var agreementsResponse = agreementsResponseTask.Result;
                var pendingAgreements = agreementsResponse?.EmployerAgreements.Where(a => a.HasPendingAgreement).Select(a => new PendingAgreementsViewModel { HashedAgreementId = a.Pending.HashedAgreementId }).ToList();

                var apprenticeshipsCount = 0;
                Task<GetApprenticeshipsResponse> apprenticeshipResponse = null;
                /*TODO : include later*/
                /*var apprenticeshipResponse = (await _commitmentsApiClient.GetApprenticeships(new GetApprenticeshipsRequest { AccountId = AccountId }))?.Apprenticeships;
                if (apprenticeshipResponse != null)
                {
                    apprenticeshipsCount = apprenticeshipResponse.Count();
                }*/

                var cohortsCount = 0;
                var draftApprenticeshipCount = 0;
                CohortSummary singleCohort = new CohortSummary();
                DraftApprenticeshipDto singleDraftApprenticeship = new DraftApprenticeshipDto();

                if (apprenticeshipResponse == null)
                {
                    var cohortsResponse = (await _commitmentsApiClient.GetCohorts(new GetCohortsRequest { AccountId = AccountId }))?.Cohorts;

                    if (cohortsResponse != null && cohortsResponse.Count() == 1)
                    {
                        cohortsCount = cohortsResponse.Count();
                        singleCohort = cohortsResponse.First();
                        draftApprenticeshipCount = singleCohort.NumberOfDraftApprentices;

                        if (draftApprenticeshipCount == 1)
                        {
                           var draftApprenticeshipsResponse = (await _commitmentsApiClient.GetDraftApprenticeships(singleCohort.CohortId))?.DraftApprenticeships;
                            singleDraftApprenticeship = draftApprenticeshipsResponse.First();
                        }
                    }
                }
                
                var viewModel = new CallToActionViewModel
                {
                    AgreementsToSign = pendingAgreements?.Count() > 0,
                    Reservations = reservationsResponse.Reservations.ToList(),
                    CohortsCount = cohortsCount,
                    ApprenticeshipsCount = apprenticeshipsCount,
                    NumberOfDraftApprentices = draftApprenticeshipCount,
                    CourseName = singleDraftApprenticeship.CourseName, //CourseName
                    CourseStartDate = singleDraftApprenticeship.StartDate, //CourseStartDate
                    CourseEndDate = singleDraftApprenticeship.EndDate, //CourseEndDate                    
                    HashedDraftApprenticeshipId = _encodingService.Encode(singleDraftApprenticeship.Id, EncodingType.ApprenticeshipId),
                    ProviderName = singleCohort.ProviderName,  //Training Provider
                    CohortStatus = singleCohort?.GetStatus() ?? CohortStatus.Unknown, //Status
                    HashedCohortReference = _encodingService.Encode(singleCohort.CohortId, EncodingType.CohortReference),
                    ApprenticeName = singleDraftApprenticeship.FirstName + " " + singleDraftApprenticeship.LastName
                };

                return new OrchestratorResponse<CallToActionViewModel>
                {
                    Status = HttpStatusCode.OK,
                    Data = viewModel
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                return new OrchestratorResponse<CallToActionViewModel>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = ex
                };
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                return new OrchestratorResponse<CallToActionViewModel>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = new ResourceNotFoundException($"An error occured whilst trying to retrieve account: {hashedAccountId}", ex)
                };
            }
            catch (Exception ex)
            {
                return new OrchestratorResponse<CallToActionViewModel>
                {
                    Status = HttpStatusCode.InternalServerError,
                    Exception = ex
                };
            }
        }
    }
}