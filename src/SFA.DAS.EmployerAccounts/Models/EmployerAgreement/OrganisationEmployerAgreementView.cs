using System;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement
{
    public class OrganisationEmployerAgreementView
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public DateTime Created { get; set; }
        public long? SignedAgreementId { get; set; }
        public int? SignedAgreementVersion { get; set; }
        public long? PendingAgreementId { get; set; }
        public int? PendingAgreementVersion { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; } //AccountLegalEntityPublicHashedId
        public DateTime? Deleted { get; set; }

        public long EmployerAgreementId { get; set; }
        public int TemplateId { get; set; }
        public EmployerAgreementStatus StatusId { get; set; }
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public long AccountLegalEntityId { get; set; }
        public long? SignedById { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public AccountAgreementType AgreementType { get; set; }
        public string PartialViewName { get; set; }
        public int VersionNumber { get; set; }

        public OrganisationType Source { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedAgreementId { get; set; }
        public string HashedLegalEntityId { get; set; }
        
        //public string PublishedMessage => GetPublishedInfo();

        //private string GetPublishedInfo()
        //{
        //    if (PartialViewName == "_Agreement_V3")
        //        return "Published 9 January 2020";
        //    if (PartialViewName == "_Agreement_V2")
        //        return "Published 1 May 2018";
        //    if (PartialViewName == "_Agreement_V3")
        //        return "Published 1 May 2017";

        //    return string.Empty; 
        //}

    }
}
