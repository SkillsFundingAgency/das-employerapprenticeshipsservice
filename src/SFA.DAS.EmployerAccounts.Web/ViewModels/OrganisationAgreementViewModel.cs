using System;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class OrganisationAgreementViewModel
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
        public string GetAgreementTabListId => $"#v{Template.VersionNumber}-agreement";
        public string GetAgreementTabPanelId => $"#v{Template.VersionNumber}-agreement";        
        public string GetAgreementTabCssClass => Template.VersionNumber == 0 ? "govuk-tabs__list-item" : "govuk-tabs__list-item govuk-tabs__list-item--selected";
        public string V2InsetText => Template.VersionNumber == 2 ? "This is a variation of the agreement that we published 1 May 2017. We updated it to add clause 7 (transfer of funding)." : "This is a new agreement.";
        
    }
}