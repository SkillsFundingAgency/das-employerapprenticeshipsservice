using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.Orchestrators
{
    public class SearchPensionRegulatorOrchestrator : UserVerificationOrchestratorBase 
    {
        private readonly ICookieStorageService<EmployerAccountData> _cookieService;
        private const string CookieName = "sfa-das-employerapprenticeshipsservice-employeraccount";

        protected SearchPensionRegulatorOrchestrator()
        {

        }

        public SearchPensionRegulatorOrchestrator(IMediator mediator, ICookieStorageService<EmployerAccountData> cookieService)
            : base(mediator)
        {
            _cookieService = cookieService;
        }

        public virtual async Task<OrchestratorResponse<SearchPensionRegulatorResultsViewModel>> SearchPensionRegulator(string payeRef)
        {
            var response = new OrchestratorResponse<SearchPensionRegulatorResultsViewModel>();

            try
            {
                var result = await Mediator.SendAsync(new GetPensionRegulatorRequest { PayeRef = payeRef });
                response.Data = new SearchPensionRegulatorResultsViewModel
                {
                    Results = CreateResult(result.Organisations).ToList(),
                    PayeRef = payeRef
                };              
            }
            catch (InvalidRequestException ex)
            {
                response.Exception = ex;
                response.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(ex.ErrorMessages);
                response.Status = HttpStatusCode.BadRequest;
            }

            return response;
        }
        
        public virtual EmployerAccountData GetCookieData()
        {
            return _cookieService.Get(CookieName);
        }                

        private IEnumerable<PensionRegulatorDetailsViewModel> CreateResult(IEnumerable<Organisation> organisations)
        {
            return organisations.Select(ConvertToViewModel);
        }
        
        private PensionRegulatorDetailsViewModel ConvertToViewModel(Organisation organisation)
        {
            return new PensionRegulatorDetailsViewModel
            {
                Name = organisation.Name,
                Status = organisation.Status,
                Address = organisation.Address.FormatAddress(),
                ReferenceNumber = organisation.UniqueIdentity
            };
        }      
    }
}