namespace SFA.DAS.EmployerAccounts.Web.ViewComponents;

public class ContinueSetupForSingleReservationViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(AccountDashboardViewModel model)
    {
        var reservation = model.CallToActionViewModel.Reservations?.FirstOrDefault();
        var viewModel = new ReservationViewModel(reservation);
        return View(viewModel);
    }
}