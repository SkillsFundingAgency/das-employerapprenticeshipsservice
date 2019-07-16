using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef;
using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Controllers.AccountPayeSchemesControllerTests
{
    [TestFixture]
    public class WhenIGetAPayeScheme : AccountPayeSchemesControllerTests
    {
        [Test]
        public async Task ThenTheAccountIsReturned()
        {
            var hashedAccountId = "ABC123";
            var payeSchemeRef = "ZZZ/123";
            var payeSchemeResponse = new GetPayeSchemeByRefResponse
            {
                PayeScheme = new PayeSchemeView
                    {
                        Ref = payeSchemeRef,
                        Name = "Test",
                        AddedDate = DateTime.Now.AddYears(-10),
                        RemovedDate = DateTime.Now
                    }
            };
            Mediator.Setup(x => x.SendAsync(It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId))).ReturnsAsync(payeSchemeResponse);

            var response = await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef.Replace("/", "%2f"));

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<OkNegotiatedContentResult<PayeSchemeViewModel>>(response);
            var model = response as OkNegotiatedContentResult<PayeSchemeViewModel>;

            model?.Content.Should().NotBeNull();
            model.Content.ShouldBeEquivalentTo(payeSchemeResponse.PayeScheme, options => options.Excluding(x => x.DasAccountId));
            model.Content.DasAccountId.Should().Be(hashedAccountId);
        }

        [Test]
        public async Task AndThePayeSchemeDoesNotExistThenItIsNotReturned()
        {
            var hashedAccountId = "ABC123";
            var payeSchemeRef = "ZZZ/123";
            var payeSchemeResponse = new GetPayeSchemeByRefResponse { PayeScheme = null };

            Mediator.Setup(x => x.SendAsync(It.Is<GetPayeSchemeByRefQuery>(q => q.Ref == payeSchemeRef && q.HashedAccountId == hashedAccountId))).ReturnsAsync(payeSchemeResponse);

            var response = await Controller.GetPayeScheme(hashedAccountId, payeSchemeRef);

            Assert.IsNotNull(response);
            Assert.IsInstanceOf<NotFoundResult>(response);
        }
    }
}
