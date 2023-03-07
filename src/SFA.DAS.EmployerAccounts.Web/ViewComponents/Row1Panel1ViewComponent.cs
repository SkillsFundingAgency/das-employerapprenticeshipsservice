namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class Row1Panel1ViewComponent: ViewComponent
{
    private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

    public Row1Panel1ViewComponent(EmployerTeamOrchestrator employerTeamOrchestrator)
    {
        _employerTeamOrchestrator = employerTeamOrchestrator;
    }

    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        var viewModel = new PanelViewModel<AccountDashboardViewModel> { ComponentName = ComponentConstants.Empty, Data = model };

        if (model.PayeSchemeCount == 0)
        {
            viewModel.ComponentName = ComponentConstants.AddPAYE;
        }
        else
        {
            _employerTeamOrchestrator.GetCallToActionViewName(viewModel);
        }

        return View(viewModel);
    }
}