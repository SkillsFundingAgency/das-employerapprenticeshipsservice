using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Formatters.Transactions.ExcelTransactionFormatterTests
{
    class WhenICreateAExcelFile
    {
        private Mock<IExcelService> _excelService;
        private ExcelTransactionFormatter _formatter;
        private TransactionDownloadLine _transactionLine;

        [SetUp]
        public void Arrange()
        {
            _excelService = new Mock<IExcelService>();
            _formatter = new ExcelTransactionFormatter(_excelService.Object);

            _transactionLine = new TransactionDownloadLine
            {
                Apprentice = "Joe Bloggs",
                PayeScheme = "123/ABCDE",
                ApprenticeTrainingCourse = "Testing",
                CohortReference = "123456",
                DateCreated = DateTime.Now,
                EmployerContribution = 12.8M,
                EnglishFraction = 11.2M,
                GovermentContribution = 23.6M,
                LevyDeclared = 12300.34M,
                PaidFromLevy = 2000.34M,
                PeriodEnd = "1617R4",
                TenPercentTopUp = 200.56M,
                Total = 345.67M,
                TrainingProvider = "Test Corp",
                TransactionType = "Levy",
                Uln = "QWERTY"
            };
        }

        [Test]
        public void ThenIShouldPassTheCorrectData()
        {
            //Arrange
            var expectedData = new[]
            {
                new[]
                {
                    "Transaction date", "Transaction type", "PAYE scheme", "Payroll month", "Levy declared",
                    "English %", "10% top up", "Training provider", "Cohort reference", "Unique learner number",
                    "Apprentice", "Apprenticeship training course", "Paid from levy", "Your contribution",
                    "Government contribution", "Total"
                },
                new[]
                {
                    _transactionLine.DateCreated.ToString(), _transactionLine.TransactionType,
                    _transactionLine.PayeScheme, _transactionLine.PeriodEnd, _transactionLine.LevyDeclared.ToString(),
                    _transactionLine.EnglishFraction.ToString(), _transactionLine.TenPercentTopUp.ToString(),
                    _transactionLine.TrainingProvider, _transactionLine.CohortReference, _transactionLine.Uln,
                    _transactionLine.Apprentice, _transactionLine.ApprenticeTrainingCourse,
                    _transactionLine.PaidFromLevy.ToString(), _transactionLine.EmployerContribution.ToString(),
                    _transactionLine.GovermentContribution.ToString(), _transactionLine.Total.ToString()
                }
            };

            //Act
            _formatter.GetFileData(new List<TransactionDownloadLine> {_transactionLine});

            //Assert
            _excelService.Verify(x => x.CreateExcelFile(It.Is<Dictionary<string, string[][]>>(t =>
                t.Values.First().First().SequenceEqual(expectedData.First()) &&
                t.Values.First().ElementAt(1).SequenceEqual(expectedData.ElementAt(1)))));
        }


        [Test]
        public void ThenIShouldReturnTheExcelFile()
        {
            //Assign
            var expectedFileData = new byte[] {1, 2, 3, 4, 5, 6};
            _excelService.Setup(x => x.CreateExcelFile(It.IsAny<Dictionary<string, string[][]>>()))
                .Returns(expectedFileData);

            //Act
            var result = _formatter.GetFileData(new List<TransactionDownloadLine> {_transactionLine});

            //Assert
            Assert.AreEqual(expectedFileData, result);
        }
    }
}
