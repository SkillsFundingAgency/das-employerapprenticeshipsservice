namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class SearchPensionRegulatorByAornViewModel : ViewModelBase
    {
        public string Aorn { get; set; }
        public string PayeRef { get; set; }

        public string AornError => GetErrorMessage(nameof(Aorn));
        public string PayeRefError => GetErrorMessage(nameof(PayeRef));
    }
}