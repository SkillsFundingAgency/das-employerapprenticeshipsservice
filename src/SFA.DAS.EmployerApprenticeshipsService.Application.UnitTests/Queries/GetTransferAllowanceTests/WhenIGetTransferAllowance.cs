using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferAllowanceTests
{
    [TestFixture]
    public class WhenIGetTransferAllowance
    {
        private const long AccountId = 111111;
        private const decimal TransferAllowance = 123.456m;

        private GetTransferAllowanceQueryHandler _handler;
        private GetTransferAllowanceQuery _query;
        private GetTransferAllowanceResponse _response;
        private Mock<EmployerFinancialDbContext> _db;
        private LevyDeclarationProviderConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _db = new Mock<EmployerFinancialDbContext>();
            _configuration = new LevyDeclarationProviderConfiguration { TransferAllowancePercentage = 10 };

            _db.Setup(d => d.SqlQueryAsync<decimal?>(It.IsAny<string>(), It.IsAny<long>(), It.IsAny<decimal>()))
                .ReturnsAsync(new List<decimal?> { TransferAllowance });

            _handler = new GetTransferAllowanceQueryHandler(_db.Object, _configuration);

            _query = new GetTransferAllowanceQuery
            {
                AccountId = AccountId
            };
        }

        [Test]
        public async Task ThenShouldGetTransferAllowance()
        {
            _response = await _handler.Handle(_query);

            _db.Verify(d => d.SqlQueryAsync<decimal?>(
                It.Is<string>(q => q.Contains("[employer_financial].[GetAccountTransferAllowance]")),
                AccountId,
                _configuration.TransferAllowancePercentage), Times.Once);
        }

        [Test]
        public async Task ThenShouldReturnGetTransferAllowanceResponse()
        {
            _response = await _handler.Handle(_query);

            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.TransferAllowance, Is.EqualTo(TransferAllowance));
        }
    }
}