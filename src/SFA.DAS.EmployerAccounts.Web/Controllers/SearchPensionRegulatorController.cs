using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerAccounts.Commands.OrganisationData;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using StructureMap.Pipeline;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts")]
    public class SearchPensionRegulatorController : BaseController
    {
        private readonly SearchPensionRegulatorOrchestrator _orchestrator;   
        private readonly IMediator _mediatr;
        private const int OrgNotListed = 0;

        public SearchPensionRegulatorController(
            IAuthenticationService owinWrapper,
            SearchPensionRegulatorOrchestrator orchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,          
            IMediator mediatr)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = orchestrator;         
            _mediatr = mediatr ?? throw new ArgumentNullException(nameof(mediatr));
        }
       
        [Route("{HashedAccountId}/pensionregulator", Order = 0)]
        [Route("pensionregulator", Order = 1)]
        public async Task<ActionResult> SearchPensionRegulator(string hashedAccountId)
        {
            var payeRef = _orchestrator.GetCookieData().EmployerAccountPayeRefData.PayeReference;
           
            if (string.IsNullOrEmpty(payeRef))
            {
                return RedirectToAction(ControllerConstants.GatewayViewName, ControllerConstants.EmployerAccountControllerName);
            }

            var model = await _orchestrator.SearchPensionRegulator(payeRef);
            model.Data.IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId);

            switch (model.Data.Results.Count)
            {
                case 0:
                {
                    return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
                }
                case 1:
                {                  
                    SavePensionRegulatorOrganisationDataIfItHasAValidName(model.Data.Results.First(), true, false);
                    return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
                }
                default:
                {
                    return View(ControllerConstants.SearchPensionRegulatorResultsViewName, model.Data);
                }
            }
        }
        
        [HttpPost]
        [Route("{HashedAccountId}/pensionregulator", Order = 0)]
        [Route("pensionregulator", Order = 1)]
        public ActionResult SearchPensionRegulator(string hashedAccountId, SearchPensionRegulatorResultsViewModel viewModel)
        {    
            if (!viewModel.SelectedOrganisation.HasValue)
            {
                ViewBag.InError = true;
                return View(ControllerConstants.SearchPensionRegulatorResultsViewName, viewModel);
            }

            if (viewModel.SelectedOrganisation == OrgNotListed)
            {
                return RedirectToAction(ControllerConstants.SearchForOrganisationActionName, ControllerConstants.SearchOrganisationControllerName);
            }

            var item = viewModel.Results.SingleOrDefault(m => m.ReferenceNumber == viewModel.SelectedOrganisation);

            if (item == null) return View(ControllerConstants.SearchPensionRegulatorResultsViewName, viewModel);

            SavePensionRegulatorOrganisationDataIfItHasAValidName(item, true, true);
            return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
        }

        [HttpGet]
        [Route("pensionregulator/aorn")]
        public async Task<ViewResult> SearchPensionRegulatorByAorn()
        {
            return View();
        }

        [HttpPost]
        [Route("pensionregulator/aorn")]
        public async Task<ActionResult> SearchPensionRegulatorByAorn(string aorn, string payeRef)
        {
            var model = await _orchestrator.GetOrganisationsByAorn(aorn, payeRef);

            switch (model.Data.Results.Count)
            {
                case 0: return View(ControllerConstants.SearchUsingAornViewName);
                case 1:
                {
                    SavePensionRegulatorOrganisationDataIfItHasAValidName(model.Data.Results.First(), true, false);
                    return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
                }
                default: return View(ControllerConstants.SearchPensionRegulatorResultsViewName, model.Data);
            }
        }

        private void SavePensionRegulatorOrganisationDataIfItHasAValidName(PensionRegulatorDetailsViewModel viewModel, bool newSearch, bool multipleResults)
        {
            if (viewModel?.Name != null)
            {
                _mediatr
                    .SendAsync(new SaveOrganisationData
                    (
                        new EmployerAccountOrganisationData
                        {
                            OrganisationReferenceNumber = viewModel.ReferenceNumber.ToString(),
                            OrganisationName = viewModel.Name,
                            OrganisationType = viewModel.Type,                       
                            OrganisationRegisteredAddress = viewModel.Address,
                            OrganisationStatus = viewModel.Status ?? string.Empty,                     
                            NewSearch = newSearch,
                            PensionsRegulatorReturnedMultipleResults = multipleResults
                        }
                    ));
            }
        }     
    }
}