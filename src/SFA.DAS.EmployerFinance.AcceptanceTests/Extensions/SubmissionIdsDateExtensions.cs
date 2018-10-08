using System;
using System.Collections.Generic;
using System.Data;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class SubmissionIdsDateExtensions
    {
        public static DataTable ToDataTable(this IDictionary<long, DateTime?> submissionIds)
        {
            var table = new DataTable();

            table.Columns.AddRange(new[]
            {
                new DataColumn("SubmissionId", typeof(long)),
                new DataColumn("CreatedDate", typeof(DateTime))
            });

            foreach (var row in submissionIds)
            {
                table.Rows.Add(row.Key, row.Value);
            }

            table.AcceptChanges();

            return table;
        }
    }
}
