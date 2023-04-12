using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisations;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators;

public class SearchOrganisationOrchestrator : UserVerificationOrchestratorBase, IOrchestratorCookie 
{
    private readonly ICookieStorageService<EmployerAccountData> _cookieService;
    private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

    public SearchOrganisationOrchestrator(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService) : base(mediator)
    {
        _cookieService = cookieService;
    }

    public async Task<OrchestratorResponse<SearchOrganisationResultsViewModel>> SearchOrganisation(string searchTerm, int pageNumber, OrganisationType? organisationType, string hashedAccountId, string userId)
    {
        var response = new OrchestratorResponse<SearchOrganisationResultsViewModel>();

        try
        {
            var result = await Mediator.Send(new GetOrganisationsRequest { SearchTerm = searchTerm, PageNumber = pageNumber, OrganisationType = organisationType });
            response.Data = new SearchOrganisationResultsViewModel
            {
                Results = CreateResult(result.Organisations),
                SearchTerm = searchTerm,
                OrganisationType = organisationType
            };

            if (!string.IsNullOrEmpty(hashedAccountId))
            {
                await SetAlreadySelectedOrganisations(hashedAccountId, userId, response.Data.Results);
            }
        }
        catch (InvalidRequestException ex)
        {
            response.Exception = ex;
            response.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(ex.ErrorMessages);
            response.Status = HttpStatusCode.BadRequest;
        }

        return response;
    }

    private async Task SetAlreadySelectedOrganisations(string hashedAccountId, string userId, PagedResponse<OrganisationDetailsViewModel> searchResults)
    {
        var accountLegalEntitiesHelper = new AccountLegalEntitiesHelper(Mediator);
        var accountLegalEntities = await accountLegalEntitiesHelper.GetAccountLegalEntities(hashedAccountId, userId);

        foreach (var searchResult in searchResults.Data)
        {
            searchResult.AddedToAccount = accountLegalEntitiesHelper.IsLegalEntityAlreadyAddedToAccount(accountLegalEntities, searchResult.Name, searchResult.ReferenceNumber, searchResult.Type);
        }
    }

    public virtual EmployerAccountData GetCookieData(HttpContext context)
    {
        return _cookieService.Get(CookieName);
    }

    public virtual void CreateCookieData(HttpContext context, EmployerAccountData data)
    {
        _cookieService.Create(data, CookieName, 365);
    }

    private static PagedResponse<OrganisationDetailsViewModel> CreateResult(PagedResponse<OrganisationName> organisations)
    {
        return new PagedResponse<OrganisationDetailsViewModel>
        {
            PageNumber = organisations.PageNumber,
            TotalPages = organisations.TotalPages,
            TotalResults = organisations.TotalResults,
            Data = organisations.Data.Select(ConvertToViewModel).ToList()
        };
    }

    private static OrganisationDetailsViewModel ConvertToViewModel(OrganisationName organisation)
    {
        return new OrganisationDetailsViewModel
        {
            Address = organisation.Address.FormatAddress(),
            Name = organisation.Name,
            Type = organisation.Type,
            DateOfInception = organisation.RegistrationDate,
            ReferenceNumber = organisation.Code,
            PublicSectorDataSource = (short?)organisation.SubType,
            Sector = organisation.Sector
        };
    }
}