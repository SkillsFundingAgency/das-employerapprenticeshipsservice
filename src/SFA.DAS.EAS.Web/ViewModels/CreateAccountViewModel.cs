using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class CreateAccountViewModel
    {
        public string UserId { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationReferenceNumber { get; set; }
        public string OrganisationAddress { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public string PayeReference { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string OrganisationStatus { get; set; }
        public string EmployerRefName { get; set; }
        public string Sector { get; set; }
    }
}