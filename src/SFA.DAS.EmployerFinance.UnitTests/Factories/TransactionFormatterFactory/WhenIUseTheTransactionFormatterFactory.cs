using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Interfaces;

namespace SFA.DAS.EmployerFinance.UnitTests.Factories.TransactionFormatterFactory
{
    public class WhenIUseTheTransactionFormatterFactory
    {
        private ITransactionFormatterFactory _paymentFormatterFactory;
        private Mock<ITransactionFormatter> _levyCsvFormatter;
        private Mock<ITransactionFormatter> _levyExcelFormatter;
        private Mock<ITransactionFormatter> _nonLevyCsvFormatter;
        private Mock<ITransactionFormatter> _nonLevyExcelFormatter;

        [SetUp]
        public void Arrange()
        {
            _levyCsvFormatter = new Mock<ITransactionFormatter>();
            _levyExcelFormatter = new Mock<ITransactionFormatter>();

            _nonLevyCsvFormatter = new Mock<ITransactionFormatter>();
            _nonLevyExcelFormatter = new Mock<ITransactionFormatter>();

            _levyCsvFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.CSV);
            _levyCsvFormatter.Setup(x => x.ApprenticeshipEmployerType).Returns(ApprenticeshipEmployerType.Levy);
            _levyExcelFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.Excel);
            _levyExcelFormatter.Setup(x => x.ApprenticeshipEmployerType).Returns(ApprenticeshipEmployerType.Levy);

            _nonLevyCsvFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.CSV);
            _nonLevyCsvFormatter.Setup(x => x.ApprenticeshipEmployerType).Returns(ApprenticeshipEmployerType.NonLevy);
            _nonLevyExcelFormatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.Excel);
            _nonLevyExcelFormatter.Setup(x => x.ApprenticeshipEmployerType).Returns(ApprenticeshipEmployerType.NonLevy);

            _paymentFormatterFactory = new EmployerFinance.Formatters.TransactionFormatterFactory(new List<ITransactionFormatter>
            {
                _levyCsvFormatter.Object,
                _levyExcelFormatter.Object,
                _nonLevyCsvFormatter.Object,
                _nonLevyExcelFormatter.Object
            });
        }

        [Test]
        public void ThenIShouldGetLevyCsvFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.CSV, ApprenticeshipEmployerType.Levy);

            Assert.AreEqual(_levyCsvFormatter.Object, formatter);
        }

        [Test]
        public void ThenIShouldGetLevyExcelFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.Excel, ApprenticeshipEmployerType.Levy);

            Assert.AreEqual(_levyExcelFormatter.Object, formatter);
        }

        [Test]
        public void ThenIShouldGetNonLevyCsvFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.CSV, ApprenticeshipEmployerType.NonLevy);

            Assert.AreEqual(_nonLevyCsvFormatter.Object, formatter);
        }

        [Test]
        public void ThenIShouldGetNonLevyExcelFormatter()
        {
            var formatter = _paymentFormatterFactory.GetTransactionsFormatterByType(DownloadFormatType.Excel, ApprenticeshipEmployerType.NonLevy);

            Assert.AreEqual(_nonLevyExcelFormatter.Object, formatter);
        }
    }
}
