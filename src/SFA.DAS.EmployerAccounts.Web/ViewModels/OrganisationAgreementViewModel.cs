using System;
using System.Linq;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class OrganisationAgreementViewModel
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
        public string AccountLegalEntityPublicHashedId { get; set; }
        public DateTime? Deleted { get; set; }

        public ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();

        public bool AgreementV1Signed => DoCheck(Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V1").FirstOrDefault());
        public bool AgreementV2Signed => DoCheck(Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V2").FirstOrDefault());
        public bool AgreementV3Signed => DoCheck(Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V3").FirstOrDefault());

        public DateTime? AgreementV1SignedDate => Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V1").FirstOrDefault().SignedDate;
        public DateTime? AgreementV2SignedDate => Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V2").FirstOrDefault().SignedDate;
        public DateTime? AgreementV3SignedDate => Agreements.Where(x => x.Template.PartialViewName == "_Agreement_V3").FirstOrDefault().SignedDate;

        public EmployerAgreementViewModelV1 EmployerAgreementV1 { get; set; }
        public EmployerAgreementViewModelV1 EmployerAgreementV2 { get; set; }
        public EmployerAgreementViewModelV1 EmployerAgreementV3 { get; set; }

        public LegalEntity LegalEntity { get; set; }

        public string HashedAccountId { get; set; }
        public string HashedLegalEntityId { get; set; }

        private bool DoCheck(EmployerAgreement employerAgreement)
        {
            return employerAgreement != null ? employerAgreement.SignedDate.HasValue : false;
        }

     
    }
}