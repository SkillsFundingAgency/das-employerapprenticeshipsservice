using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Levy;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetLevyDeclarationsByAccountAndPeriodTests
{
    public class WhenIGetLevyDeclarations
    {
        private Mock<IDasLevyRepository> _repository;
        private Mock<IHashingService> _hashingService;
        private GetLevyDeclarationsByAccountAndPeriodQueryHandler _handler;

        [SetUp]
        public void Arrange()
        {
            _repository = new Mock<IDasLevyRepository>();
            _hashingService = new Mock<IHashingService>();

            _handler = new GetLevyDeclarationsByAccountAndPeriodQueryHandler(_repository.Object, _hashingService.Object);
        }

        [Test]
        public async Task ThenLevyDeclarationsAreReturned()
        {
            var accountId = 123;
            var query = new GetLevyDeclarationsByAccountAndPeriodRequest { HashedAccountId = "ABC123", PayrollMonth = 3, PayrollYear = "2017-18" };
            _hashingService.Setup(x => x.DecodeValue(query.HashedAccountId)).Returns(accountId);
            var expectedDeclarations = new List<LevyDeclarationItem>();
            _repository.Setup(x => x.GetAccountLevyDeclarations(accountId, query.PayrollYear, query.PayrollMonth)).ReturnsAsync(expectedDeclarations);

            var response = await _handler.Handle(query);

            response.Declarations.Should().BeSameAs(expectedDeclarations);
        }
    }
}
