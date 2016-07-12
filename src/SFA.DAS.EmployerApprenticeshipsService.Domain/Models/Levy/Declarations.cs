using System.Collections.Generic;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy
{
    public class Declarations
    {
        public string empref { get; set; }
        public string schemeCessationDate { get; set; }
        public List<Declaration> declarations { get; set; }
    }

    public class PayrollMonth
    {
        public int year { get; set; }
        public int month { get; set; }
    }

    public class Declaration
    {
        public string submissionType { get; set; }
        public string submissionDate { get; set; }
        public PayrollMonth payrollMonth { get; set; }
        public int amount { get; set; }
    }
    
}