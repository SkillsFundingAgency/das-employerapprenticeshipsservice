using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Api.Types;
using SFA.DAS.EmployerFinance.Queries.GetLevyDeclaration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SFA.DAS.EmployerFinance.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    [TestFixture]
    public class WhenIGetLevyForAnAccount : AccountLevyControllerTests
    {
        [Test]
        public async Task ThenTheLevyIsReturned()
        {
            //Arrange
            var hashedAccountId = "ABC123";
            var levyResponse = new GetLevyDeclarationResponse { Declarations = LevyDeclarationItems.Create(12334, "abc123") };
            Mediator.Setup(x => x.SendAsync(It.Is<GetLevyDeclarationRequest>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(levyResponse);           

            //Act
            var response = await Controller.Index(hashedAccountId);

            //Assert
            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<List<LevyDeclaration>>>(response);
            var model = response as OkNegotiatedContentResult<List<LevyDeclaration>>;

            model?.Content.Should().NotBeNull();
            Assert.IsTrue(model?.Content.TrueForAll(x => x.HashedAccountId == hashedAccountId));
            model?.Content.ShouldAllBeEquivalentTo(levyResponse.Declarations, options => options.Excluding(x => x.HashedAccountId).Excluding(x => x.PayeSchemeReference));
            Assert.IsTrue(model?.Content[0].PayeSchemeReference == levyResponse.Declarations[0].EmpRef);
        }
    }
}
