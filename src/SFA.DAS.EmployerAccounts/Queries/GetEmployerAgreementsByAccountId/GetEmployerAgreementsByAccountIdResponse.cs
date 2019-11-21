using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementsByAccountId
{
    public class GetEmployerAgreementsByAccountIdResponse
    {
        public List<EmployerAgreement> EmployerAgreements { get; set; }
    }
}