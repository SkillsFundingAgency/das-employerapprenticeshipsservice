using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Extensions;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Commands.RefreshEmployerLevyData
{
    public class LevyImportCleanupStrategy : ILevyImportCleanupStrategy
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly IHmrcDateService _hmrcDateService;
        private readonly ILog _logger;
        public LevyImportCleanupStrategy(
            IDasLevyRepository dasLevyRepository,
            IHmrcDateService hmrcDateService,
            ILog logger)
        {
            _dasLevyRepository = dasLevyRepository;
            _hmrcDateService = hmrcDateService;
            _logger = logger;
        }
        public async Task<IEnumerable<DasDeclaration>> Cleanup(string empRef, IEnumerable<DasDeclaration> declarations)
        {
            var temp = declarations.OrderBy(c => c.SubmissionDate).ToArray();

            temp = FilterDuplicateHmrcDeclarations(empRef, temp);

            temp = await FilterActiveDeclarations(empRef, temp);

            ProcessNoPaymentForPeriodDeclarations(empRef, temp);

            await ProcessEndOfYearAdjustmentDeclarations(empRef, temp);

            return temp;
        }

        /// <summary> 
        /// If there are any Submissions from Hmrc that have the same submission Id, we should discard all 
        /// but the first. 
        /// </summary> 
        private DasDeclaration[] FilterDuplicateHmrcDeclarations(string empRef, IEnumerable<DasDeclaration> declarations)
        {
            var temp = declarations as ICollection<DasDeclaration> ?? declarations.ToArray();

            var duplicateIds = temp
                .GroupBy(d => d.Id)
                .Where(g => g.Count() > 1)
                .Select(s => s.Key)
                .ToList();

            if (duplicateIds.Any())
            {
                _logger.Info($"PayeScheme '{empRef}' has duplicate submission id(s) from Hmrc = '{string.Join(", ", duplicateIds)}'");
            }

            return temp.DistinctBy(x => x.Id).ToArray();
        }

        private async Task ProcessEndOfYearAdjustmentDeclarations(string empRef, DasDeclaration[] declarations)
        {
            var endOfYearAdjustmentDeclarations = declarations.Where(IsEndOfYearAdjustment).ToList();

            foreach (var dasDeclaration in endOfYearAdjustmentDeclarations)
            {
                await UpdateEndOfYearAdjustment(empRef, dasDeclaration, declarations);
            }
        }

        private static void ProcessNoPaymentForPeriodDeclarations(string empRef, DasDeclaration[] declarations)
        {
            var noPaymentForPeriodDeclarations = declarations.Where(x => x.NoPaymentForPeriod);

            foreach (var dasDeclaration in noPaymentForPeriodDeclarations)
            {
                dasDeclaration.LevyDueYtd = null;
            }
        }

        private async Task<DasDeclaration[]> FilterActiveDeclarations(string empRef, DasDeclaration[] declarations)
        {
            var existingSubmissionIds = await _dasLevyRepository.GetEmployerDeclarationSubmissionIds(empRef);
            var existingSubmissionIdsLookup = new HashSet<string>(existingSubmissionIds.Select(x => x.ToString()));

            //NOTE: The submissionId in our database is the same as the declaration ID from HMRC (DasDeclaration)
            declarations = declarations.Where(x => !existingSubmissionIdsLookup.Contains(x.Id)).ToArray();

            declarations = declarations.Where(x => !DoesSubmissionPreDateTheLevy(x)).ToArray();

            return declarations.Where(x => !IsSubmissionForFuturePeriod(x)).ToArray();
        }
        private bool DoesSubmissionPreDateTheLevy(DasDeclaration dasDeclaration)
        {
            return _hmrcDateService.DoesSubmissionPreDateLevy(dasDeclaration.PayrollYear);
        }

        private bool IsSubmissionForFuturePeriod(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionForFuturePeriod(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, DateTime.UtcNow);
        }

        private bool IsEndOfYearAdjustment(DasDeclaration dasDeclaration)
        {
            return dasDeclaration.PayrollMonth.HasValue && _hmrcDateService.IsSubmissionEndOfYearAdjustment(dasDeclaration.PayrollYear, dasDeclaration.PayrollMonth.Value, dasDeclaration.SubmissionDate);
        }

        private async Task UpdateEndOfYearAdjustment(string empRef, DasDeclaration yearEndAdjustment, DasDeclaration[] hmrcDeclarations)
        {
            if (yearEndAdjustment.LevyDueYtd == null && !yearEndAdjustment.NoPaymentForPeriod)
            {
                throw new ArgumentNullException(nameof(yearEndAdjustment));
            }

            yearEndAdjustment.EndOfYearAdjustment = true;

            if (yearEndAdjustment.NoPaymentForPeriod)
                return;

            var period12Declaration = await GetDeclarationEffectiveForPeriod12(empRef, yearEndAdjustment.PayrollYear, yearEndAdjustment.SubmissionDate, hmrcDeclarations);

            if (period12Declaration?.LevyDueYtd != null)
            {
                // Caution: the calculation here is incorrect (it should be the other way round). This results in negative values in LevyDeclaration EndOfYearAdjustmentAmount.
                // However, do not fix this as somewhere later (probably in a view or sproc) the EndOfYearAdjustmentAmount is being inverted when writing to the transaction line table.
                yearEndAdjustment.EndOfYearAdjustmentAmount =
                    period12Declaration.LevyDueYtd.Value - yearEndAdjustment.LevyDueYtd ?? 0;
            }
            else
            {
                yearEndAdjustment.EndOfYearAdjustmentAmount = yearEndAdjustment.LevyDueYtd ?? 0;
            }
        }

        /// <summary>
        ///     Returns the latest declaration for the year (which is the one that will be effective for period 12).
        /// </summary>
        /// <param name="hmrcDeclarations">The declarations retrieved from HMRC (these are in-memory and have not yet been written to the database.</param>
        /// <returns>The declaration that was effective for period 12 or null if one was not found.</returns>
        /// <remarks>
        ///     The database contains all the declarations retrieved to date from HMRC, <see cref="hmrcDeclarations"/>
        ///     contains the declarations that HMRC has that we have not retrieved previously (which is determined by submission date).
        /// </remarks>
        private async Task<DasDeclaration> GetDeclarationEffectiveForPeriod12(string empRef, string payrollYear, DateTime yearEndAdjustmentCutOff, IEnumerable<DasDeclaration> hmrcDeclarations)
        {
            // Look in the declarations that have just been retrieved from HMRC (which will contain everything not yet in the database)
            DasDeclaration period12Declaration = await GetEffectivePeriod12SubmissionFromLatestHmrcFeed(hmrcDeclarations, payrollYear, yearEndAdjustmentCutOff);

            // Look in the database (which will contain everything previously retrieved from HMRC)
            if (period12Declaration == null)
            {
                period12Declaration = await GetEffectivePeriod12SubmissionFromDatabase(empRef, payrollYear, yearEndAdjustmentCutOff);
            }

            return period12Declaration;
        }

        private Task<DasDeclaration> GetEffectivePeriod12SubmissionFromLatestHmrcFeed(IEnumerable<DasDeclaration> declarations, string payrollYear, DateTime yearEndAdjustmentCutOff)
        {
            // We are only interested in declarations for the current year that are not year-end-adjustments and that were submitted on time
            DasDeclaration period12Declaration = declarations
                .Where(ld => ld.PayrollYear == payrollYear
                             && ld.PayrollMonth.HasValue
                             // We're interested in LD that are either within the month (i.e. pre-cut off) or are year end adjustments before the supplied cut-off date
                             && ((!ld.EndOfYearAdjustment && _hmrcDateService.IsDateInPayrollPeriod(ld.PayrollYear, ld.PayrollMonth.Value, ld.SubmissionDate))
                                || ld.EndOfYearAdjustment && ld.SubmissionDate < yearEndAdjustmentCutOff)) 
                .OrderByDescending(ld => ld.PayrollMonth)
                .ThenByDescending(ld => ld.SubmissionDate)
                .FirstOrDefault();

            return Task.FromResult(period12Declaration);
        }

        private Task<DasDeclaration> GetEffectivePeriod12SubmissionFromDatabase(string empRef, string payrollYear, DateTime yearEndAdjustmentCutOff)
        {
            return _dasLevyRepository.GetEffectivePeriod12Declaration(empRef, payrollYear, yearEndAdjustmentCutOff);
        }
    }
}