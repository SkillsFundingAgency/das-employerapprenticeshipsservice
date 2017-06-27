using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetOrganisations;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.EAS.Domain.Models.ReferenceData;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class SearchOrganisationOrchestrator : UserVerificationOrchestratorBase
    {
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        public SearchOrganisationOrchestrator(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService)
            : base(mediator)
        {
            _cookieService = cookieService;
        }

        public async Task<OrchestratorResponse<SearchOrganisationViewModel>> SearchOrganisation(string searchTerm, int pageNumber, OrganisationType? organisationType)
        {
            var response = new OrchestratorResponse<SearchOrganisationViewModel>();

            try
            {
                var result = await Mediator.SendAsync(new GetOrganisationsRequest { SearchTerm = searchTerm, PageNumber = pageNumber, OrganisationType = organisationType });
                response.Data = new SearchOrganisationViewModel
                {
                    Results = ConvertToViewModel(result.Organisations),
                    SearchTerm = searchTerm,
                    OrganisationType = organisationType
                };
            }
            catch (InvalidRequestException ex)
            {
                response.Exception = ex;
                response.FlashMessage = new FlashMessageViewModel().CreateErrorFlashMessageViewModel(ex.ErrorMessages);
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }

        public virtual EmployerAccountData GetCookieData(HttpContextBase context)
        {
            return _cookieService.Get(CookieName);
        }

        public virtual void CreateCookieData(HttpContextBase context, EmployerAccountData data)
        {
            _cookieService.Create(data, CookieName, 365);
        }

        private PagedResponse<OrganisationDetailsViewModel> ConvertToViewModel(PagedResponse<Organisation> organisations)
        {
            return new PagedResponse<OrganisationDetailsViewModel>
            {
                PageNumber = organisations.PageNumber,
                TotalPages = organisations.TotalPages,
                TotalResults = organisations.TotalResults,
                Data = organisations.Data.Select<Organisation, OrganisationDetailsViewModel>(ConvertToViewModel).ToList()
            };
        }

        private OrganisationDetailsViewModel ConvertToViewModel(Organisation organisation)
        {
            return new OrganisationDetailsViewModel
            {
                Address = organisation.Address.GetAddress(),
                Name = organisation.Name,
                Type = organisation.Type,
                DateOfInception = organisation.RegistrationDate,
                OrganisationCode = organisation.Code,
                PublicSectorDataSource = (short?)organisation.SubType,
                Sector = organisation.Sector
            };
        }
    }
}