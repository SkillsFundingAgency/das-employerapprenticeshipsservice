using System;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Models.PAYE
{
    public class PayeSchemeViewModel : IAccountResource
    {
        public string DasAccountId { get; set; }
        public string Ref { get; set; }
        public string Name { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? RemovedDate { get; set; }
    }
}