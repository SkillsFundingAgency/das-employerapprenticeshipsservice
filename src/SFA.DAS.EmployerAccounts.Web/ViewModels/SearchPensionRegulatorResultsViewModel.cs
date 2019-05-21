using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class SearchPensionRegulatorResultsViewModel
    {
        public string PayeRef { get; set; }
        public IList<PensionRegulatorDetailsViewModel> Results { get; set; }
        public bool IsExistingAccount { get; set; }
        public string SelectedOrganisation { get; set; }
    }
}