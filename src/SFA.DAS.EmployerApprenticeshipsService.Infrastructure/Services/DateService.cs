using System;
using System.Collections.Concurrent;
using DocumentFormat.OpenXml.Wordprocessing;
using SFA.DAS.EAS.Infrastructure.Interfaces;
using SFA.DAS.EAS.Infrastructure.Models;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public class DateService : IDateService
    {
        private readonly ConcurrentDictionary<int, FinancialYearDetails> _yearDetails = new ConcurrentDictionary<int, FinancialYearDetails>();
        private readonly IFinancialYearDateCalculator _financialYearDateCalculator;
        private readonly ISystemDateService _systemDateService;

        public DateService(IFinancialYearDateCalculator financialYearDateCalculator, ISystemDateService systemDateService)
        {
            _financialYearDateCalculator = financialYearDateCalculator;
            _systemDateService = systemDateService;
        }

        public DateTime CurrentSystemTime => _systemDateService.Current;

        public FinancialYearDetails PreviousFinancialYear
        {
            get
            {
                var endYear = _financialYearDateCalculator.GetEndFinancialYear(CurrentSystemTime) - 1;
                return GetFinancialYear(endYear);
            }
        }

        public FinancialYearDetails CurrentFinancialYear
        {
            get
            {
                var endYear = _financialYearDateCalculator.GetEndFinancialYear(CurrentSystemTime);
                return GetFinancialYear(endYear);
            }
        }

        public FinancialYearDetails NextFinancialYear
        {
            get
            {
                var endYear = _financialYearDateCalculator.GetEndFinancialYear(CurrentSystemTime) + 1;
                return GetFinancialYear(endYear);
            }
        }

        public FinancialYearDetails GetFinancialYearStarting(int startingInYear)
        {
            return GetFinancialYear(startingInYear+1);
        }

        public FinancialYearDetails GetFinancialYear(DateTime atPointInTime)
        {
            var endYear = _financialYearDateCalculator.GetEndFinancialYear(atPointInTime);
            return GetFinancialYear(endYear);
        }

        public FinancialYearDetails GetFinancialYearEnding(int endingInYear)
        {
            return GetFinancialYear(endingInYear);
        }

        private FinancialYearDetails GetFinancialYear(int endYear)
        {
            return _yearDetails.GetOrAdd(endYear, BuildYearDetails);
        }

        private FinancialYearDetails BuildYearDetails(int endYear)
        {
            return new FinancialYearDetails(
                endYear - 1, 
                endYear, 
                _financialYearDateCalculator.GetYearStart(endYear),
                _financialYearDateCalculator.GetYearEnd(endYear));
        }
    }
}