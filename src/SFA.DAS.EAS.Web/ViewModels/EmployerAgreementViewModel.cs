using SFA.DAS.EAS.Domain.Models.EmployerAgreement;
using System;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class EmployerAgreementViewModel
    {
        public EmployerAgreementView EmployerAgreement { get; set; }

        public EmployerAgreementView PreviouslySignedEmployerAgreement => new EmployerAgreementView()
        {
            HashedAccountId = "ABC123",
            HashedAgreementId = "DEF456",
            SignedByName = "Test Person",
            SignedDate = DateTime.Now,
            LegalEntityName = "Test Corp"
        };
    }

}