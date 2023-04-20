using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.EmployerAccounts.Commands.OrganisationData;
using SFA.DAS.EmployerAccounts.Infrastructure;
using SFA.DAS.EmployerAccounts.Models.ReferenceData;
using SFA.DAS.EmployerAccounts.Web.Authentication;

namespace SFA.DAS.EmployerAccounts.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasEmployerViewerTransactorOwnerAccount))]
[SetNavigationSection(NavigationSection.AccountsAgreements)]
[Route("accounts")]
public class SearchOrganisationController : BaseController
{
    private readonly SearchOrganisationOrchestrator _orchestrator;
    //This is temporary until the existing add org function is replaced, at which point the method used can be moved to the org search orchestrator
    private readonly IMediator _mediator;
    

    public SearchOrganisationController(
        SearchOrganisationOrchestrator orchestrator,
        ICookieStorageService<FlashMessageViewModel> flashMessage,
        IMediator mediator)
        : base( flashMessage)
    {
        _orchestrator = orchestrator;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [Route("{HashedAccountId}/organisations/search", Order = 0)]
    [Route("organisations/search", Order = 1)]
    public IActionResult SearchForOrganisation(string hashedAccountId)
    {
        var model = new OrchestratorResponse<SearchOrganisationViewModel> { Data = new SearchOrganisationViewModel { IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId) } };
        return View(ControllerConstants.SearchForOrganisationViewName, model);
    }

    [HttpPost]
    [Route("{HashedAccountId}/organisations/search", Order = 0)]
    [Route("organisations/search", Order = 1)]
    public IActionResult SearchForOrganisation(string hashedAccountId, string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
        {
            var model = CreateSearchTermValidationErrorModel(new SearchOrganisationViewModel { IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId) });
            return View(ControllerConstants.SearchForOrganisationViewName, model);
        }

        return RedirectToAction(ControllerConstants.SearchForOrganisationResultsActionName, new { hashedAccountId, searchTerm });
    }

    [Route("{HashedAccountId}/organisations/search/results", Order = 0)]
    [Route("organisations/search/results", Order = 1)]
    public async Task<IActionResult> SearchForOrganisationResults(string hashedAccountId, string searchTerm, int pageNumber = 1, OrganisationType? organisationType = null)
    {
        OrchestratorResponse<SearchOrganisationResultsViewModel> model;
        if (string.IsNullOrEmpty(searchTerm))
        {
            var viewModel = new SearchOrganisationResultsViewModel { Results = new PagedResponse<OrganisationDetailsViewModel>() };
            model = CreateSearchTermValidationErrorModel(viewModel);
        }
        else
        {
            model = await _orchestrator.SearchOrganisation(searchTerm, pageNumber, organisationType, hashedAccountId, HttpContext.User.FindFirstValue(EmployerClaims.IdamsUserIdClaimTypeIdentifier));
        }
        model.Data.IsExistingAccount = !string.IsNullOrEmpty(hashedAccountId);

        return View(ControllerConstants.SearchForOrganisationResultsViewName, model);
    }

    [HttpPost]
    [Route("{HashedAccountId}/organisations/search/confirm", Order = 0)]
    [Route("organisations/search/confirm", Order = 1)]
    public IActionResult Confirm(string hashedAccountId, OrganisationDetailsViewModel viewModel)
    {
        viewModel.NewSearch = true;

        SaveOrganisationDataIfItHasAValidName(viewModel);

        if (string.IsNullOrEmpty(hashedAccountId))
        {
            return RedirectToAction(ControllerConstants.SummaryActionName, ControllerConstants.EmployerAccountControllerName);
        }

        var response = new OrchestratorResponse<OrganisationDetailsViewModel> { Data = viewModel };

        return View(ControllerConstants.ConfirmOrganisationDetailsViewName, response);
    }

    private void SaveOrganisationDataIfItHasAValidName(OrganisationDetailsViewModel viewModel)
    {
        if (viewModel?.Name != null)
        {
            _mediator.Send(new SaveOrganisationData
                (
                    new EmployerAccountOrganisationData
                    {
                        OrganisationType = viewModel.Type,
                        OrganisationReferenceNumber = viewModel.ReferenceNumber,
                        OrganisationName = viewModel.Name,
                        OrganisationDateOfInception = viewModel.DateOfInception,
                        OrganisationRegisteredAddress = viewModel.Address,
                        OrganisationStatus = viewModel.Status ?? string.Empty,
                        PublicSectorDataSource = viewModel.PublicSectorDataSource,
                        Sector = viewModel.Sector,
                        NewSearch = viewModel.NewSearch
                    }
                ));
        }
    }

    private static OrchestratorResponse<T> CreateSearchTermValidationErrorModel<T>(T data)
    {
        var model = new OrchestratorResponse<T> { Data = data };
        SetSearchTermValidationModelProperties(model);
        return model;
    }

    private static void SetSearchTermValidationModelProperties(OrchestratorResponse model)
    {
        model.Status = HttpStatusCode.BadRequest;
        model.FlashMessage = FlashMessageViewModel.CreateErrorFlashMessageViewModel(new Dictionary<string, string> { { "searchTerm", "Enter organisation name" } });
    }
}