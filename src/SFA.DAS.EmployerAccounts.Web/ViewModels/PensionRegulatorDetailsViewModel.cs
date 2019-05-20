using System;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class PensionRegulatorDetailsViewModel : NavigationViewModel
    {
        public string HashedId { get; set; }

        [AllowHtml]
        public string Name { get; set; }
        public string Address { get; set; }
        public DateTime? DateOfInception { get; set; }
        public string ReferenceNumber { get; set; }
        public string Status { get; set; }
        public bool AddedToAccount { get; set; }
        public string NameError => GetErrorMessage(nameof(Name));
        public string Sector { get; set; }
        public bool NewSearch { get; set; }
    }
}