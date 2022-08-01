using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetLevyDeclaration;
using SFA.DAS.EAS.TestCommon.ObjectMothers;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountLevyControllerTests
{
    [TestFixture]
    public class WhenIGetLevyForAnAccount : AccountLevyControllerTests
    {
        
        [Test]
        public async Task ThenTheLevyIsReturned()
        {
            var hashedAccountId = "ABC123";
            var levyResponse = new GetLevyDeclarationResponse { Declarations = LevyDeclarationViewsObjectMother.Create(12334, "abc123") };
            Mediator.Setup(x => x.SendAsync(It.Is<GetLevyDeclarationRequest>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(levyResponse);
            //FinanceApiService.Setup(x => x.GetLevyDeclarations(hashedAccountId)).ReturnsAsync(ICollection<LevyDeclarationViewModel>);           

            var fixture = new Fixture();
            ICollection<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel> apiResponse = new List<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel>()
            {
                 fixture.Create<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel>(),
                fixture.Create<SFA.DAS.EAS.Finance.Api.Types.LevyDeclarationViewModel>()
            };
            FinanceApiService.Setup(x => x.GetLevyDeclarations(hashedAccountId)).ReturnsAsync(apiResponse);

            //FinanceApiService.Setup(x => x.GetLevyDeclarations(hashedAccountId)).ReturnsAsync((ICollection<Finance.Api.Types.LevyDeclarationViewModel>)It.IsAny<ICollection<LevyDeclarationViewModel>>());

            var response = await Controller.Index(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>>(response);
            var model = response as OkNegotiatedContentResult<AccountResourceList<SFA.DAS.EAS.Account.Api.Types.LevyDeclarationViewModel>>;

            model?.Content.Should().NotBeNull();
            Assert.IsTrue(model?.Content.TrueForAll(x => x.HashedAccountId == hashedAccountId));
            //model?.Content.ShouldAllBeEquivalentTo(levyResponse.Declarations, options => options.Excluding(x => x.HashedAccountId).Excluding(x => x.PayeSchemeReference));
            //Assert.IsTrue(model?.Content[0].PayeSchemeReference == levyResponse.Declarations[0].EmpRef);
        }

        [Test]
        public async Task AndTheAccountDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var levyResponse = new GetLevyDeclarationResponse { Declarations = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetLevyDeclarationRequest>(q => q.HashedAccountId == hashedAccountId))).ReturnsAsync(levyResponse);

            var response = await Controller.Index(hashedAccountId);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }   
}
