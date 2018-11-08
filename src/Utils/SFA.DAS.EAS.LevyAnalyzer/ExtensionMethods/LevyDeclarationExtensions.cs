using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public class PeriodDeclarations
    {
        public PayrollPeriod PayrollPeriod { get; set; }
        public LevyDeclaration[] Declarations { get; set; }
    }

    public static class LevyDeclarationExtensions
    {
        /// <summary>
        ///     Remove all period 12 declarations from the input stream.
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludePeriod12(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(levy => levy.PayrollMonth < 12);
        }

        public static bool IsValid(this LevyDeclaration declaration)
        {
            return declaration.PayrollMonth.HasValue &&
                   !string.IsNullOrWhiteSpace(declaration.PayrollYear) &&
                   declaration.LevyDueYTD.HasValue &&
                   declaration.SubmissionDate.HasValue;
        }

        /// <summary>
        ///     Remove all declarations that cannot be processed because either payroll period or amount was not supplied by HMRC
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludeInvalidDeclarations(
            this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(IsValid);
        }

        /// <summary>
        ///     Remove all declarations that would be considered late. Period 12 are never late.
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludeLateSubmissions(
            this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            return declarations
                .Where(declaration => declaration.IsValid() &&
                                      (declaration.PayrollMonth.Value == 12 ||
                                       hmrcDateService.IsDateInPayrollPeriod(declaration.PayrollYear,
                                           declaration.PayrollMonth.Value, declaration.SubmissionDate.Value)));
        }

        /// <summary>
        ///     Returns active transactions (which are those that are valid, on-time and not-superseded)
        /// </summary>
        public static IEnumerable<LevyDeclaration> ActiveDeclarations(
            this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            foreach (var periodDeclarations in declarations.ExcludeLateSubmissions(hmrcDateService).GroupByPayrollPeriod())
            {
                if (periodDeclarations.PayrollPeriod.PayrollMonth == 12)
                {
                    foreach (var period12Declaration in periodDeclarations.Declarations)
                    {
                        yield return period12Declaration;
                    }
                }
                else
                {
                    yield return periodDeclarations.Declarations.Last();
                }
            }
        }

        private static readonly PayrollPeriodComparer PayrollPeriodComparer = new PayrollPeriodComparer();

        /// <summary>
        ///     Returns declarations grouped by payroll period.
        /// </summary>
        public static IEnumerable<PeriodDeclarations> GroupByPayrollPeriod(
            this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations
                .Where(IsValid)
                .GroupBy(levy => new PayrollPeriod(levy.PayrollYear, levy.PayrollMonth.Value), PayrollPeriodComparer)
                .Select(grp => new PeriodDeclarations
                {
                    PayrollPeriod = grp.Key,
                    Declarations = grp.OrderBy(declaration => declaration.SubmissionDate).ToArray()
                });
        }

        /// <summary>
        ///     Returns the monthly declared values from the list of declarations.
        /// </summary>
        public static IEnumerable<CalculatedLevyDeclation> CalculateMonthlyValues(this IEnumerable<LevyDeclaration> levyDeclarations)
        {
            LevyDeclaration previousDeclaration = null;

            foreach (var declaration in levyDeclarations)
            {
                var result = new CalculatedLevyDeclation(declaration, previousDeclaration);

                previousDeclaration = declaration;

                yield return result;
            }
        }
    }
}
