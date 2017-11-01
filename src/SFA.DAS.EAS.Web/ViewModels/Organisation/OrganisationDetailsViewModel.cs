using System;
using System.Web.Mvc;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class OrganisationDetailsViewModel : NavigationViewModel
    {
        public string HashedId { get; set; }
        public OrganisationType Type { get; set; }
        public short? PublicSectorDataSource { get; set; }

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