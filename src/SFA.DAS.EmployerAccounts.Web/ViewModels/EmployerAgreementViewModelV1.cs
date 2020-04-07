using System;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class EmployerAgreementViewModelV1
    {
        public long Id { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public long? SignedById { get; set; }
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public long AccountLegalEntityId { get; set; }
        public AccountLegalEntity AccountLegalEntity { get; set; }
        public EmployerAgreementStatus StatusId { get; set; }
        public AgreementTemplate Template { get; set; }
        public int TemplateId { get; set; }
        public bool OrganisationLookupPossible { get; set; }
        public string HashedAccountId { get; set; }
        public string HashedLegalEntityId { get; set; }
        public string HashedAgreementId { get; set; }

        public string AccountLegalEntityPublicHashedId => AccountLegalEntity.PublicHashedId;

        public string SignedDateText { get; set; }
    }
}