using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerFinance.Types.Models
{
    public class ExpiredFunds : IExpiredFunds
    {
        public decimal GetExpiringFundsByDate(
            IDictionary<CalendarPeriod, decimal> fundsIn,
            IDictionary<CalendarPeriod, decimal> fundsOut,
            DateTime date,
            IDictionary<CalendarPeriod, decimal> expired, int expiryPeriod)
        {
            var expiredFunds = GetExpiringFunds(fundsIn, fundsOut, expired, expiryPeriod);

            if (!expiredFunds.Any())
            {
                return 0;
            }

            var expiredFundsKey = expiredFunds.Keys.SingleOrDefault(key => key.Year.Equals(date.Year) && key.Month.Equals(date.Month));

            return expiredFundsKey != null ? expiredFunds[expiredFundsKey] : 0;
        }

        public IDictionary<CalendarPeriod, decimal> GetExpiringFunds(
            IDictionary<CalendarPeriod, decimal> fundsIn,
            IDictionary<CalendarPeriod, decimal> fundsOut,
            IDictionary<CalendarPeriod, decimal> expired, int expiryPeriod)
        {
            if (fundsIn == null)
            {
                throw new ArgumentNullException(nameof(fundsIn));
            }

            if (fundsOut == null)
            {
                throw new ArgumentNullException(nameof(fundsOut));
            }

            CalculateAndApplyAdjustmentsToTotals(fundsOut);

            CalculateAndApplyAdjustmentsToTotals(fundsIn);

            CalculateAndApplyExpiredFundsToFundsOut(fundsOut, expired, fundsIn, expiryPeriod);

            var expiredFunds = CalculatedExpiredFunds(fundsIn, fundsOut, expired, expiryPeriod);

            return expiredFunds;
        }

        private static void CalculateAndApplyAdjustmentsToTotals(IDictionary<CalendarPeriod, decimal> calendarFunds)
        {
            if (!calendarFunds.Any(c => c.Value < 0))
            {
                return;
            }

            var adjustments = calendarFunds.Where(c => c.Value < 0)
                                       .ToDictionary(key => key.Key, value => value.Value);

            foreach (var adjustment in adjustments.OrderBy(c => c.Key))
            {
                var adjustmentAmount = adjustment.Value * -1;

                var orderedAdjustments = calendarFunds.Where(c => c.Value > 0)
                                          .Where(c => c.Key <= adjustment.Key)
                                          .ToDictionary(c => c.Key, c => c.Value)
                                          .OrderByDescending(c => c.Key);

                foreach (var orderedAdjustment in orderedAdjustments)
                {
                    if (orderedAdjustment.Value >= adjustmentAmount)
                    {
                        calendarFunds[orderedAdjustment.Key] = orderedAdjustment.Value - adjustmentAmount;
                        break;
                    }

                    if (orderedAdjustment.Value >= adjustmentAmount)
                    {
                        continue;
                    }

                    calendarFunds[orderedAdjustment.Key] = 0;
                    adjustmentAmount = adjustmentAmount - orderedAdjustment.Value;
                }
            }
        }

        private static void CalculateAndApplyExpiredFundsToFundsOut(
            IDictionary<CalendarPeriod, decimal> fundsOut,
            IDictionary<CalendarPeriod, decimal> expired,
            IDictionary<CalendarPeriod, decimal> fundsIn,
                int expiryPeriod)
        {
            if (expired == null)
            {
                return;
            }

            foreach (var expiredAmount in expired)
            {
                var levyPeriodForExpiry =
                    new DateTime(expiredAmount.Key.Year, expiredAmount.Key.Month, 1).AddMonths(expiryPeriod * -1);
                var fundsInAmount = fundsIn.SingleOrDefault(c => c.Key.Equals(new CalendarPeriod(levyPeriodForExpiry.Year, levyPeriodForExpiry.Month))).Value;
                var amount = (fundsInAmount >= 0 ? fundsInAmount : 0) - expiredAmount.Value;

                if (amount <= 0)
                {
                    continue;
                }

                var fundsOutAvailable = fundsOut
                    .Where(c => c.Value > 0 && c.Key <= expiredAmount.Key)
                    .ToList();

                foreach (var fundOut in fundsOutAvailable)
                {
                    if (fundOut.Value >= amount)
                    {
                        fundsOut[fundOut.Key] = fundOut.Value - amount;
                        break;
                    }
                    amount = amount - fundOut.Value;
                    fundsOut[fundOut.Key] = 0;
                }
            }
        }

        private static decimal CalculateExpiryAmount(IDictionary<CalendarPeriod, decimal> fundsOut, DateTime expiryDate, decimal expiryAmount)
        {
            var fundsOutAvailable = fundsOut
                .Where(c => new DateTime(c.Key.Year, c.Key.Month, 1) <= expiryDate && c.Value > 0)
                .ToList();

            if (!fundsOutAvailable.Any())
            {
                return expiryAmount;
            }

            foreach (var spentFunds in fundsOutAvailable)
            {
                if (spentFunds.Value >= expiryAmount)
                {
                    fundsOut[spentFunds.Key] = spentFunds.Value - expiryAmount;
                    expiryAmount = 0;
                    break;
                }

                expiryAmount = expiryAmount - spentFunds.Value;
                fundsOut[spentFunds.Key] = 0;
            }

            return expiryAmount;
        }

        private static IDictionary<CalendarPeriod, decimal> CalculatedExpiredFunds(
            IDictionary<CalendarPeriod, decimal> fundsIn,
            IDictionary<CalendarPeriod, decimal> fundsOut,
            IDictionary<CalendarPeriod, decimal> expired,
            int expiryPeriod)
        {
            var expiredFunds = new Dictionary<CalendarPeriod, decimal>();

            foreach (var fundsInPair in fundsIn.OrderBy(c => c.Key))
            {
                var expiryDateOfFundsIn = new DateTime(fundsInPair.Key.Year, fundsInPair.Key.Month, 1)
                                                    .AddMonths(expiryPeriod);

                var amountDueToExpire = fundsInPair.Value >= 0 ? fundsInPair.Value : 0;

                var alreadyExpiredAmount = expired?.Keys.SingleOrDefault(c => c.Year.Equals(expiryDateOfFundsIn.Year)
                                                                             && c.Month.Equals(expiryDateOfFundsIn.Month));

                if (alreadyExpiredAmount != null)
                {
                    amountDueToExpire = expired[alreadyExpiredAmount];
                }
                else
                {
                    amountDueToExpire = amountDueToExpire > 0
                        ? CalculateExpiryAmount(fundsOut, expiryDateOfFundsIn, amountDueToExpire)
                        : 0;
                }

                expiredFunds.Add(new CalendarPeriod(expiryDateOfFundsIn.Year, expiryDateOfFundsIn.Month), amountDueToExpire);
            }

            return expiredFunds;
        }
    }
}