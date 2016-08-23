using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain
{
    public class LevyDeclarationView
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string EmpRef { get; set; }
        public DateTime SubmissionDate { get; set; }
        public long SubmissionId { get; set; }
        public decimal Amount { get; set; }
        public decimal EnglishFraction { get; set; }
        public string PayrollYear { get; set; }
        public int PayrollMonth { get; set; }

        public string PayrollDate()
        {
            var year = 2000;
            var month = 1;

            if (PayrollMonth <= 9)
            {
                year += Convert.ToInt32(PayrollYear.Split('-')[0]);
                PayrollMonth = PayrollMonth + 3;
            }
            else
            {
                year += Convert.ToInt32(PayrollYear.Split('-')[1]);
                PayrollMonth = PayrollMonth - 9;
            }

            var dateTime = new DateTime(year, month,1);
            
            return dateTime.ToString("MMM yyyy");
        }
    }
}