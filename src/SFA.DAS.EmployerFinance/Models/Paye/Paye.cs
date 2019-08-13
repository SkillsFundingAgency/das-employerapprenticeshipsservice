using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Models.Paye
{
    public class Paye
    {
        [Key]
        public string Ref { get; set; }
        public long AccountId { get; set; }  
        public string RefName { get; set; }

        public Paye(string empRef, long accountId, string name)
        {
            Ref = empRef;
            AccountId = accountId;
            RefName = name;
        }

        public Paye() { }
    }
}