using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerFinance.Models.Paye
{
    public class Paye
    {
        [Key]
        public string EmpRef { get; set; }
        public long AccountId { get; set; }  
        public string Name { get; set; }
        public string Aorn { get; set; }

        public Paye(string empRef, long accountId, string name, string aorn)
        {
            EmpRef = empRef;
            AccountId = accountId;
            Name = name;
            Aorn = aorn;
        }

        public Paye() { }
    }
}