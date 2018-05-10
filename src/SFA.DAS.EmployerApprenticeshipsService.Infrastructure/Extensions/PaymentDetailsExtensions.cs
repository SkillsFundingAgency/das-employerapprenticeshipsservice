using System;
using System.Collections.Generic;
using System.Data;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class PaymentDetailsExtensions
    {
        public static DataTable ToPaymentsDataTable(this IEnumerable<PaymentDetails> payments)
        {
            var paymentsDataTable = new DataTable();

            paymentsDataTable.Columns.Add("PaymentId", typeof(Guid));
            paymentsDataTable.Columns.Add("Ukprn", typeof(long));
            paymentsDataTable.Columns.Add("ProviderName", typeof(string));
            paymentsDataTable.Columns.Add("Uln", typeof(long));
            paymentsDataTable.Columns.Add("AccountId", typeof(long));
            paymentsDataTable.Columns.Add("ApprenticeshipId", typeof(long));
            paymentsDataTable.Columns.Add("DeliveryPeriodMonth", typeof(int));
            paymentsDataTable.Columns.Add("DeliveryPeriodYear", typeof(int));
            paymentsDataTable.Columns.Add("CollectionPeriodId", typeof(string));
            paymentsDataTable.Columns.Add("CollectionPeriodMonth", typeof(int));
            paymentsDataTable.Columns.Add("CollectionPeriodYear", typeof(int));
            paymentsDataTable.Columns.Add("EvidenceSubmittedOn", typeof(DateTime));
            paymentsDataTable.Columns.Add("EmployerAccountVersion", typeof(string));
            paymentsDataTable.Columns.Add("ApprenticeshipVersion", typeof(string));
            paymentsDataTable.Columns.Add("FundingSource", typeof(string));
            paymentsDataTable.Columns.Add("TransactionType", typeof(string));
            paymentsDataTable.Columns.Add("Amount", typeof(decimal));
            paymentsDataTable.Columns.Add("PeriodEnd", typeof(string));
            paymentsDataTable.Columns.Add("StandardCode", typeof(long));
            paymentsDataTable.Columns.Add("FrameworkCode", typeof(int));
            paymentsDataTable.Columns.Add("ProgrammeType", typeof(int));
            paymentsDataTable.Columns.Add("PathwayCode", typeof(int));
            paymentsDataTable.Columns.Add("PathwayName", typeof(string));
            paymentsDataTable.Columns.Add("ApprenticeshipCourseName", typeof(string));
            paymentsDataTable.Columns.Add("ApprenticeName", typeof(string));
            paymentsDataTable.Columns.Add("ApprenticeNINumber", typeof(string));
            paymentsDataTable.Columns.Add("ApprenticeshipCourseLevel", typeof(int));
            paymentsDataTable.Columns.Add("ApprenticeshipCourseStartDate", typeof(DateTime));

            foreach (var payment in payments)
            {
                paymentsDataTable.Rows.Add(
                    payment.Id,
                    payment.Ukprn,
                    payment.ProviderName,
                    payment.Uln,
                    payment.EmployerAccountId,
                    payment.ApprenticeshipId,
                    payment.DeliveryPeriodMonth,
                    payment.DeliveryPeriodYear,
                    payment.CollectionPeriodId,
                    payment.CollectionPeriodMonth,
                    payment.CollectionPeriodYear,
                    payment.EvidenceSubmittedOn,
                    payment.EmployerAccountVersion,
                    payment.ApprenticeshipVersion,
                    payment.FundingSource,
                    payment.TransactionType,
                    payment.Amount,
                    payment.PeriodEnd,
                    payment.StandardCode,
                    payment.FrameworkCode,
                    payment.ProgrammeType,
                    payment.PathwayCode,
                    payment.PathwayName,
                    payment.CourseName,
                    payment.ApprenticeName,
                    payment.ApprenticeNINumber,
                    payment.CourseLevel,
                    payment.CourseStartDate);
            }

            return paymentsDataTable;
        }
    }
}