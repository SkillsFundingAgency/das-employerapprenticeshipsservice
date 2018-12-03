﻿using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.UnitTests.Factories.TransactionFormatterFactory
{
    public class WhenIUseTheTransactionFormatterFactory
    {
        private ITransactionFormatterFactory _paymentFormatterFactory;
        private Mock<ITransactionFormatter> _csvFormatter;
        private Mock<ITransactionFormatter> _excelFormatter;


       [SetUp]
        public void Arrange()
        {
            _csvFormatter = new Mock<ITransactionFormatter>();
            _excelFormatter = new Mock<ITransactionFormatter>();

            _csvFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.CSV);
            _excelFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.Excel);

            _paymentFormatterFactory = new EmployerFinance.Formatters.TransactionFormatterFactory(new List<ITransactionFormatter>
            {
                _csvFormatter.Object,
                _excelFormatter.Object
            });
        }

        [Test]
        public void ThenIShouldGetCsvFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.CSV);

            Assert.AreEqual(_csvFormatter.Object, formatter);
        }

        [Test]
        public void ThenIShouldGetExcelFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.Excel);

            Assert.AreEqual(_excelFormatter.Object, formatter);
        }
    }
}
