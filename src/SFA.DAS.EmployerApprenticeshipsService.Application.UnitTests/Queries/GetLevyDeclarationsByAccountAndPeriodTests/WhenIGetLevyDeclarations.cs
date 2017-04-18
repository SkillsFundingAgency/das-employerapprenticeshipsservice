using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclarationsByAccountAndPeriod;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetLevyDeclarationsByAccountAndPeriodTests
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
            var expectedDeclarations = new List<LevyDeclarationView>();
            _repository.Setup(x => x.GetAccountLevyDeclarations(accountId, query.PayrollYear, query.PayrollMonth)).ReturnsAsync(expectedDeclarations);

            var response = await _handler.Handle(query);

            response.Declarations.Should().BeSameAs(expectedDeclarations);
        }
    }
}
