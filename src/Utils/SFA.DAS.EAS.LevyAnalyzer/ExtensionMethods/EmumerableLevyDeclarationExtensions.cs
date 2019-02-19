using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.LevyAnalyser.Models;
using SFA.DAS.EAS.LevyAnalyser.Rules;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EAS.LevyAnalyser.ExtensionMethods
{
    public static class EmumerableLevyDeclarationExtensions
    {
        /// <summary>
        ///     Remove all period 12 declarations from the input stream.
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludePeriod12(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(levy => levy.PayrollMonth < 12);
        }

        /// <summary>
        ///     Retain only period 12 declarations from the input stream.
        /// </summary>
        public static IEnumerable<LevyDeclaration> OnlyPeriod12(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(levy => levy.PayrollMonth == 12);
        }

        /// <summary>
        ///     Remove all declarations that cannot be processed because they are incomplete
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludeInvalidDeclarations(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations.Where(declaration => declaration.IsValid());
        }

        /// <summary>
        ///     Remove all declarations that would be considered late. Period 12 are never late.
        /// </summary>
        public static IEnumerable<LevyDeclaration> ExcludeLateSubmissions(
            this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            return declarations
                .Where(declaration => declaration.IsValid() &&
                                      (declaration.PayrollMonth.Value == 12 || !declaration.IsLate(hmrcDateService))).ToList();
        }

        /// <summary>
        ///     Remove all declarations that were not late.
        /// </summary>
        public static IEnumerable<LevyDeclaration> OnlyLateSubmissions(
            this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            return declarations
                .Where(declaration => declaration.IsValid() && declaration.IsLate(hmrcDateService));
        }

        /// <summary>
        ///     Returns active transactions (which are those that are valid, on-time and not-superseded)
        /// </summary>
        /// <remarks>
        ///     There is a problem with the logic behind the way the system attempts to handle superseded declarations:
        //      If an employer submits two declarations for a period (doesn't matter which period) and
        //      both these are submitted on time. 
        //      If these are processed as a single batch (i.e. in the same feed from HMRC) then we will end up with 
        //      one transaction in the database. If these are processed separately then we will end up with two
        //      transactions in the database. So the situation in the database is unpredictable.
        /// </remarks>
        public static IEnumerable<LevyDeclaration> ActiveDeclarations(
            this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            var periodDeclarations = declarations
                .ExcludeLateSubmissions(hmrcDateService)
                .GroupByPayrollPeriod();

            foreach (var periodDeclaration in periodDeclarations)
            {
                // we want to exclude all but the last on-time submission
                if (periodDeclaration.PayrollPeriod.PayrollMonth == 12)
                {
                    foreach (var levyDeclaration in HandlePeriod12Declarations(hmrcDateService, periodDeclaration))
                    {
                        yield return levyDeclaration;
                    }
                }
                else
                {
                    yield return periodDeclaration.Declarations.Last();
                }
            }
        }

        public static IEnumerable<LevyDeclaration> ExcludeAllButLastEndOfYearAdjustment(this IEnumerable<LevyDeclaration> declarations, IHmrcDateService hmrcDateService)
        {
            return declarations
                .Where(x => x.PayrollMonth != 12 || (x.PayrollMonth == 12 && x.IsOntime(hmrcDateService))).Union(
                    declarations.Where(x => x.PayrollMonth == 12 && x.IsLate(hmrcDateService))
                        .OrderByDescending(x => x.SubmissionDate).Take(1));
        }

        private static IEnumerable<LevyDeclaration> HandlePeriod12Declarations(
            IHmrcDateService hmrcDateService, 
            PeriodDeclarations periodDeclaration)
        {
            LevyDeclaration lastOntime = null;

            // Period 12 declarations are handled differently - we skip all the on-time
            // declarations to keep just the last (just like normal periods) but then
            // we append all the P12 adjustments.
            foreach (var period12Declaration in periodDeclaration.Declarations)
            {
                if (period12Declaration.IsOntime(hmrcDateService))
                {
                    lastOntime = period12Declaration;
                }
                else
                {
                    if (lastOntime != null)
                    {
                        yield return lastOntime;
                        lastOntime = null;
                    }

                    yield return period12Declaration;
                }
            }

            if (lastOntime != null)
            {
                yield return lastOntime;
            }
        }

        /// <summary>
        ///     Returns transactions group by year 
        /// </summary>
        /// <param name="declarations"></param>
        /// <returns></returns>
        public static IEnumerable<YearDeclarations> GroupByYear(this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations
                .GroupBy(declaration => declaration.PayrollYear)
                .Select(year =>new YearDeclarations
                {
                    PayrollYear = year.Key,
                    Declarations = year.OrderBy(declaration => declaration.SubmissionDate).ToArray()
                });
        }

        private static readonly PayrollPeriodComparer PayrollPeriodComparer = new PayrollPeriodComparer();

        /// <summary>
        ///     Returns declarations grouped by payroll period.
        /// </summary>
        public static IEnumerable<PeriodDeclarations> GroupByPayrollPeriod(
            this IEnumerable<LevyDeclaration> declarations)
        {
            return declarations
                .Where(declaration => declaration.IsValid())
                .GroupBy(levy => new PayrollPeriod(levy.PayrollYear, levy.PayrollMonth.Value), PayrollPeriodComparer)
                .Select(grp => new PeriodDeclarations
                {
                    PayrollPeriod = grp.Key,
                    Declarations = grp.OrderBy(declaration => declaration.SubmissionDate).ToArray()
                });
        }

        public static IEnumerable<LevyDeclaration> OnlyYearEndAdjustments(this IEnumerable<LevyDeclaration> declarations,
            IHmrcDateService hmrcDateService)
        {
            return declarations.OnlyPeriod12().OnlyLateSubmissions(hmrcDateService);
        }

        /// <summary>
        ///     Returns the monthly declared values from the list of declarations. This is calculated as the difference between
        ///     the current month and the preceding month.
        /// </summary>
        public static IEnumerable<CalculatedLevyDeclation> CalculateMonthlyValues(
            this IEnumerable<LevyDeclaration> levyDeclarations)
        {
            LevyDeclaration previousDeclaration = null;

            foreach (var declaration in levyDeclarations)
            {
                var result = new CalculatedLevyDeclation(declaration, previousDeclaration);

                previousDeclaration = declaration;

                yield return result;
            }
        }

        /// <summary>
        ///     Returns the declarations that are considered to be year end adjustments.
        /// </summary>
        public static IEnumerable<CalculatedYearEndAdjustment> CalculateYearEndAdjustments(
            this IEnumerable<LevyDeclaration> levyDeclarations, IHmrcDateService hmrcDateService)
        {
            var yearDeclarations = levyDeclarations.ActiveDeclarations(hmrcDateService).GroupByYear();

            foreach (var yearDeclaration in yearDeclarations)
            {
                LevyDeclaration previousDeclaration = yearDeclaration.Declarations.LastOrDefault(declaration => declaration.IsOntime(hmrcDateService)); 

                foreach (var yearEndAdjustment in yearDeclaration.Declarations.OnlyYearEndAdjustments(hmrcDateService)) 
                {
                    var result = new CalculatedYearEndAdjustment(yearEndAdjustment, previousDeclaration);

                    previousDeclaration = yearEndAdjustment;

                    yield return result;
                }
            }
        }
    }
}