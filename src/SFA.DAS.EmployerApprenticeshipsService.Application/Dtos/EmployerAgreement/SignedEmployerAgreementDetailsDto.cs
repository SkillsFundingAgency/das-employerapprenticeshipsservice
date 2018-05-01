using System;

namespace SFA.DAS.EAS.Application.Dtos.EmployerAgreement
{
    public class SignedEmployerAgreementDetailsDto : EmployerAgreementDetails
    {
        public string SignedByName { get; set; }
        public DateTime? SignedDate { get; set; }
        public DateTime? ExpiredDate { get; set; }
    }
}