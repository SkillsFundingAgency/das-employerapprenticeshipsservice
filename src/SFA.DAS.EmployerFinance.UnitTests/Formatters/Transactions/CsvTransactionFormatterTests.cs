using System;
using System.Globalization;
using System.Text;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;

namespace SFA.DAS.EmployerFinance.UnitTests.Formatters.Transactions
{
    public class CsvTransactionFormatterTests : BaseFormatterTest
    {
        public override ITransactionFormatter PaymentFormatter => new CsvTransactionFormatter();

        public override string ExpectedMimeType => "text/csv";

        public override string ExpectedFileExtension => "csv";

        public override DownloadFormatType ExpectedDownloadFormats => DownloadFormatType.CSV;

        [Test]
        public void WhenICallGetContentsIGetCorrectHeaderFormat()
        {
            var formattedFileData = PaymentFormatter.GetFileData(TransactionDownloadLines);

            var formattedFileContent = Encoding.UTF8.GetString(formattedFileData);

            var rows = formattedFileContent.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            // We should have 1 header and 3 data rows
            Assert.AreEqual(4, rows.Length);

            // We should have a header row in a specific format
            var headerColumns = rows[0].Split(char.Parse(","));
            
            Assert.AreEqual("Transaction date", headerColumns[0]);
            Assert.AreEqual("Transaction type", headerColumns[1]);
            Assert.AreEqual("PAYE scheme", headerColumns[2]);
            Assert.AreEqual("Payroll month", headerColumns[3]);
            Assert.AreEqual("Levy declared", headerColumns[4]);
            Assert.AreEqual("English %", headerColumns[5]);
            Assert.AreEqual("10% top up", headerColumns[6]);
            Assert.AreEqual("Training provider", headerColumns[7]);
            Assert.AreEqual("Unique learner number", headerColumns[8]);
            Assert.AreEqual("Apprentice", headerColumns[9]);
            Assert.AreEqual("Apprenticeship training course", headerColumns[10]);
            Assert.AreEqual("Paid from levy", headerColumns[11]);
            Assert.AreEqual("Your contribution", headerColumns[12]);
            Assert.AreEqual("Government contribution", headerColumns[13]);
            Assert.AreEqual("Total", headerColumns[14]);
        }

        [Test]
        public void WhenICallGetContentsIGetCorrectDataFormat()
        {
            var formattedFileData = PaymentFormatter.GetFileData(TransactionDownloadLines); 

            var formattedFileContent = Encoding.UTF8.GetString(formattedFileData);

            var rows = formattedFileContent.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.None
            );

            // We should have 1 header and 3 data rows
            Assert.AreEqual(4, rows.Length);
            
            var i = 1m;
            while (i < 4)
            {
                // We should be able to extract known values from each row
                var dataRow = rows[Convert.ToInt32(i)].Split(char.Parse(","));

                Assert.AreEqual(DateTime.Today.AddMonths(Convert.ToInt32(-i)).ToString("dd/MM/yyyy"), dataRow[0]);
                Assert.AreEqual($"{TransactionTypePrefix}{i}", dataRow[1]);
                Assert.AreEqual($"{EmpRefPrefix}{i}", dataRow[2]);
                Assert.AreEqual($"{PeriodEndPrefix}{i}", dataRow[3]);
                Assert.AreEqual((i * 1000).ToString("0.00", CultureInfo.CurrentCulture), dataRow[4]); // LevyDeclared
                Assert.AreEqual((i * 10).ToString("0.000", CultureInfo.CurrentCulture), dataRow[5]);
                Assert.AreEqual((i * 100).ToString("0.00", CultureInfo.CurrentCulture), dataRow[6]);
                Assert.AreEqual($"{TrainingProviderPrefix}{i}", dataRow[7]);
                Assert.AreEqual($"{UlnPrefix}{i}", dataRow[8]);
                Assert.AreEqual($"{ApprenticePrefix}{i}", dataRow[9]);
                Assert.AreEqual($"{ApprenticeTrainingCoursePrefix}{i}", dataRow[10]);
                Assert.AreEqual((i * 10).ToString("0.00", CultureInfo.CurrentCulture), dataRow[11]);
                Assert.AreEqual((i).ToString("0.00", CultureInfo.CurrentCulture), dataRow[12]);
                Assert.AreEqual((i * 10000).ToString("0.00", CultureInfo.CurrentCulture), dataRow[13]);
                Assert.AreEqual(((i* 1000) + (i * 100)).ToString("0.00", CultureInfo.CurrentCulture), dataRow[14]);
                i++;
            }
        }
    }
}
