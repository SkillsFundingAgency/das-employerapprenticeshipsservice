using System;
using SFA.DAS.EAS.Domain.Models.Organisation;

namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class OrganisationViewModelBase : ViewModelBase
    {
        public string HashedAccountId { get; set; }
        public string OrganisationHashedId { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationReferenceNumber { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public string OrganisationStatus { get; set; }
    }
}