using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class Agreement
    {
        public long Id { get; set; }
        public DateTime? SignedDate { get; set; }
        public string SignedByName { get; set; }
        public long? SignedById { get; set; }
        public string SignedByEmail { get; set; }
        public EmployerAgreementStatus Status { get; set; }
        public int TemplateVersionNumber { get; set; }
        public AgreementType AgreementType { get; set; }
    }
}