using System.Data;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.Extensions
{
    public static class SubmissionIdsExtensions
    {
        public static DataTable ToDataTable(this long[] submissionIds)
        {
            var table = new DataTable();

            table.Columns.AddRange(new[]
            {
                new DataColumn("SubmissionId", typeof(long))
            });

            foreach (var submissionId in submissionIds)
            {
                table.Rows.Add(submissionId);
            }

            table.AcceptChanges();

            return table;
        }

    }
}
