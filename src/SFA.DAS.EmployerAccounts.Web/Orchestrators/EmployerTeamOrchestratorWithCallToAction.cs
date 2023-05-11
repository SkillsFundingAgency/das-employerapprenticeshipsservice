using AutoMapper;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class EmployerTeamOrchestratorWithCallToAction : EmployerTeamOrchestrator
{
    internal const string AccountContextCookieName = "sfa-das-employerapprenticeshipsservice-accountcontext";
    private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;
    private readonly ICookieStorageService<AccountContext> _accountContext;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<EmployerTeamOrchestratorWithCallToAction> _logger;
    private readonly IEncodingService _encodingService;

    public EmployerTeamOrchestratorWithCallToAction(
        EmployerTeamOrchestrator employerTeamOrchestrator,
        IMediator mediator,
        ICurrentDateTime currentDateTime,
        IAccountApiClient accountApiClient,
        IMapper mapper,
        ICookieStorageService<AccountContext> accountContext,
        ILogger<EmployerTeamOrchestratorWithCallToAction> logger,
        EmployerAccountsConfiguration configuration,
        IEncodingService encodingService)
        : base(mediator, currentDateTime, accountApiClient, mapper, configuration, encodingService)
    {
        _employerTeamOrchestrator = employerTeamOrchestrator;
        _accountContext = accountContext;
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
        _encodingService = encodingService;
    }

    //Needed for tests	
    protected EmployerTeamOrchestratorWithCallToAction() { }

    public override async Task<OrchestratorResponse<AccountDashboardViewModel>> GetAccount(string hashedAccountId, string externalUserId)
    {
        var accountResponseTask = _employerTeamOrchestrator.GetAccount(hashedAccountId, externalUserId);

        if (TryGetAccountContext(hashedAccountId, out var accountContext) && accountContext.ApprenticeshipEmployerType == ApprenticeshipEmployerType.Levy)
        {
            var levyResponse = await accountResponseTask;
            SaveContext(levyResponse);
            return levyResponse;
        }

        // here we are either non levy or unknown caller context
        var accountResponse = await accountResponseTask;
        var callToActionResponse = await GetCallToAction(hashedAccountId, externalUserId);
      
        if (accountResponse.Status == HttpStatusCode.OK)
        {
            if (callToActionResponse.Status == HttpStatusCode.OK)
            {
                accountResponse.Data.CallToActionViewModel = callToActionResponse.Data;
            }
            else
            {
                _logger.LogError(callToActionResponse.Exception, "An error occurred whilst trying to retrieve account CallToAction: {HashedAccountId}", hashedAccountId);
            }
        }

        SaveContext(accountResponse);
        return accountResponse;
    }

    private void SaveContext(OrchestratorResponse<AccountDashboardViewModel> orchestratorResponse)
    {
        if (orchestratorResponse.Status == HttpStatusCode.OK)
        {
            _accountContext.Delete(AccountContextCookieName);

            _accountContext.Create(
                new AccountContext
                {
                    HashedAccountId = orchestratorResponse.Data.HashedAccountId,
                    ApprenticeshipEmployerType = orchestratorResponse.Data.ApprenticeshipEmployerType
                }
                , AccountContextCookieName);
        }
    }

    private bool TryGetAccountContext(string hashedAccountId, out AccountContext accountContext)
    {
        if (_accountContext.Get(AccountContextCookieName) is AccountContext accountCookie && accountCookie.HashedAccountId.Equals(hashedAccountId, StringComparison.InvariantCultureIgnoreCase))
        {
            accountContext = accountCookie;
            return true;
        }

        accountContext = null;
        return false;
    }

    private async Task<OrchestratorResponse<CallToActionViewModel>> GetCallToAction(string hashedAccountId, string externalUserId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        try
        {
            var reservationsResponseTask = _mediator.Send(new GetReservationsRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            var apprenticeshipsResponseTask = _mediator.Send(new GetApprenticeshipsRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            var accountCohortResponseTask = _mediator.Send(new GetSingleCohortRequest
            {
                AccountId = accountId,
                ExternalUserId = externalUserId
            });

            var vacanciesResponseTask = _mediator.Send(new GetVacanciesRequest
            {
                HashedAccountId = hashedAccountId,
                ExternalUserId = externalUserId
            });

            await Task.WhenAll(reservationsResponseTask, vacanciesResponseTask, apprenticeshipsResponseTask, accountCohortResponseTask).ConfigureAwait(false);

            var reservationsResponse = reservationsResponseTask.Result;
            var vacanciesResponse = vacanciesResponseTask.Result;
            var apprenticeshipsResponse = apprenticeshipsResponseTask.Result;
            var accountCohortResponse = accountCohortResponseTask.Result;

            CallToActionViewModel viewModel = null;
            var status = HttpStatusCode.OK;

            if (vacanciesResponse.HasFailed || reservationsResponse.HasFailed || accountCohortResponse.HasFailed || apprenticeshipsResponse.HasFailed)
            {
                status = HttpStatusCode.InternalServerError;
            }
            else
            {
                viewModel = new CallToActionViewModel
                {
                    Reservations = reservationsResponse.Reservations?.ToList(),
                    VacanciesViewModel = new VacanciesViewModel
                    {
                        Vacancies = _mapper.Map<IEnumerable<Vacancy>, IEnumerable<VacancyViewModel>>(vacanciesResponse.Vacancies)
                    },
                    Apprenticeships = _mapper.Map<IEnumerable<Apprenticeship>, IEnumerable<ApprenticeshipViewModel>>(apprenticeshipsResponse?.Apprenticeships),
                    Cohorts = accountCohortResponse.Cohort != null
                        ? new List<CohortViewModel> { _mapper.Map<Cohort, CohortViewModel>(accountCohortResponse.Cohort) }
                        : new List<CohortViewModel>()
                };
            }

            return new OrchestratorResponse<CallToActionViewModel>
            {
                Status = status,
                Data = viewModel
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