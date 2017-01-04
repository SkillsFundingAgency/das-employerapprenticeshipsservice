using System;

namespace SFA.DAS.EAS.Api.Models
{
    public class AccountInformationViewModel
    {
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OrganisationRegisteredAddress { get; set; }
        public string OrganisationSource { get; set; }
        public string OrganisationStatus { get; set; }
        public string OrganisationName { get; set; }
        public DateTime OrgansiationCreatedDate { get; set; }
        public string OwnerEmail { get; set; }
        public string DasAccountId { get; set; }
        public string OrganisationNumber { get; set; }
    }
}