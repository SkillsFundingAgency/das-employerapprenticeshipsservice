using System;
using System.Collections.Generic;
using System.Globalization;
using HMRC.ESFA.Levy.Api.Types;
using TechTalk.SpecFlow;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class LevyDeclarationsExtensions
    {
        /// <summary>
        /// Converts the table data into a LevyDeclarations Object
        /// </summary>
        /// <param name="levyDeclarations">The convereted object.</param>
        /// <param name="table">The source data table</param>
        /// <returns>The submission ids that were converted</returns>
        public static Dictionary<long, DateTime?> ImportData(this LevyDeclarations levyDeclarations, Table table)
        {
            var submissionIds = new Dictionary<long, DateTime?>();

            foreach (var tableRow in table.Rows)
            {
                var noPaymentForPeriod = false;
                if (tableRow.ContainsKey("NoPaymentForPeriod"))
                {
                    if (!string.IsNullOrWhiteSpace(tableRow["NoPaymentForPeriod"]))
                        noPaymentForPeriod = Convert.ToBoolean(tableRow["NoPaymentForPeriod"]);
                }

                var submissionId = long.Parse(tableRow["Id"]);

                levyDeclarations.Declarations.Add(new Declaration
                {
                    Id = submissionId.ToString(),
                    SubmissionId = submissionId,
                    NoPaymentForPeriod = noPaymentForPeriod,
                    PayrollPeriod = new PayrollPeriod
                    {
                        Month = Convert.ToInt16(tableRow["Payroll_Month"]),
                        Year = tableRow["Payroll_Year"]
                    },
                    DateCeased = null,
                    InactiveFrom = null,
                    InactiveTo = null,
                    LevyAllowanceForFullYear = 0,
                    LevyDeclarationSubmissionStatus = LevyDeclarationSubmissionStatus.LatestSubmission,
                    LevyDueYearToDate = Convert.ToDecimal(tableRow["LevyDueYtd"]),
                    SubmissionTime = DateTime.Parse(tableRow["SubmissionDate"])
                });

                DateTime? createdDate = null;
                if (tableRow.ContainsKey("CreatedDate") && tableRow["CreatedDate"] != null)
                {
                    createdDate = DateTime.ParseExact(tableRow["CreatedDate"], "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);
                }

                submissionIds.Add(submissionId, createdDate);
            }

            return submissionIds;
        }
    }
}
