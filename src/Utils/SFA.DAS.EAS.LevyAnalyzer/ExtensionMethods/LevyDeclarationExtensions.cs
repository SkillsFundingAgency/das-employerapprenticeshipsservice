using System;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public static class LevyDeclarationExtensions
    {

        public static bool IsLate(this LevyDeclaration declaration, IHmrcDateService hmrcDateService)
        {
            var periodEndDate = hmrcDateService.GetDateRangeForPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value).EndDate;
            return declaration.SubmissionDate.Value > periodEndDate;
        }

        public static bool IsOntime(this LevyDeclaration declaration, IHmrcDateService hmrcDateService)
        {
            return hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value,
                declaration.SubmissionDate.Value); ;
        }

        public static bool IsValid(this LevyDeclaration declaration)
        {
            return declaration.PayrollMonth.HasValue &&
                   !string.IsNullOrWhiteSpace(declaration.PayrollYear) &&
                   declaration.LevyDueYTD.HasValue &&
                   declaration.SubmissionDate.HasValue;
        }
    }
}