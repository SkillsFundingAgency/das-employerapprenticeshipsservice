using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public static class LevyDeclarationExtensions
    {
        public static IEnumerable<LevyDeclaration> ExcludePeriod12(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(levy => levy.PayrollMonth < 12);
        }

        public static IEnumerable<LevyDeclaration> ExcludeLateSubmissions(this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            foreach (var declaration in declarations)
            {
                // Period 12 cannot be later, as late period 12 are adjustments
                if (declaration.PayrollMonth.Value == 12 || hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear, declaration.PayrollMonth.Value, declaration.SubmissionDate.Value))
                {
                    yield return declaration;
                }
            }
        }

        private static readonly PayrollPeriodComparer _payrollPeriodComparer = new PayrollPeriodComparer();

        public static IEnumerable<IGrouping<PayrollPeriod, LevyDeclaration>> GroupByPayrollPeriod(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.GroupBy(levy => new PayrollPeriod(levy.PayrollYear, levy.PayrollMonth.Value), _payrollPeriodComparer);
        }
    }
}
