using SFA.DAS.EAS.Domain.Models.Payments;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class PaymentDetailsExtensions
    {
        public static void AssertValidPayment(this PaymentDetails payment)
        {
            /*
                Possible truncations:
                CollectionPeriodId          20
                EmployerAccountVersion      50  
                ApprenticeshipVersion       25
                PeriodEnd                   25
            */
            StringBuilder errors = null;
            var isOkay = CheckFieldLength(payment.CollectionPeriodId, nameof(payment.CollectionPeriodId), 20, false, ref errors) &&
                         CheckFieldLength(payment.EmployerAccountVersion, nameof(payment.EmployerAccountVersion), 50, false, ref errors) &&
                         CheckFieldLength(payment.ApprenticeshipVersion, nameof(payment.ApprenticeshipVersion), 25, false, ref errors) &&
                         CheckFieldLength(payment.PeriodEnd, nameof(payment.PeriodEnd), 25, false, ref errors);

            if (!isOkay)
            {
                throw new Exception($"The following errors were detected:{errors.ToString()}");
            }
        }

        private static bool CheckFieldLength(this string field, string fieldName, int maxLength, bool isNullAllowed, ref StringBuilder errors)
        {
            if (field == null)
            {
                if (isNullAllowed)
                {
                    return true;
                }

                AddError($"Field:{fieldName} Error:null value is not allowed", ref errors);
                return false;
            }

            if (field.Length > maxLength)
            {
                AddError($"Field:{fieldName} Error:field is {field.Length} but maximum length is {maxLength}", ref errors);
                return false;
            }

            return true;
        }

        private static void AddError(string error, ref StringBuilder errors)
        {
            if (errors == null)
            {
                errors = new StringBuilder();
            }

            errors.AppendLine(error);
        }

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
            paymentsDataTable.Columns.Add("CollectionPeriodId", typeof(string)).MaxLength = 20;
            paymentsDataTable.Columns.Add("CollectionPeriodMonth", typeof(int));
            paymentsDataTable.Columns.Add("CollectionPeriodYear", typeof(int));
            paymentsDataTable.Columns.Add("EvidenceSubmittedOn", typeof(DateTime));
            paymentsDataTable.Columns.Add("EmployerAccountVersion", typeof(string)).MaxLength = 50;
            paymentsDataTable.Columns.Add("ApprenticeshipVersion", typeof(string)).MaxLength = 25;
            paymentsDataTable.Columns.Add("FundingSource", typeof(string));
            paymentsDataTable.Columns.Add("TransactionType", typeof(string));
            paymentsDataTable.Columns.Add("Amount", typeof(decimal));
            paymentsDataTable.Columns.Add("PeriodEnd", typeof(string)).MaxLength = 25;
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
                AssertValidPayment(payment);

                paymentsDataTable.Rows.Add(
                    Guid.Parse(payment.Id),
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
                    ((int)payment.FundingSource).ToString(),
                    ((int)payment.TransactionType).ToString(),
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