using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferAllowanceTests
{
    public class WhenIValidateTheRequest
    {
        private GetTransferAllowanceRequestValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                                 .ReturnsAsync(new MembershipView());

            _validator = new GetTransferAllowanceRequestValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenTheRequestIsInvalidIfTheFieldsAreNotPopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetTransferAllowanceRequest { HashedAccountId = string.Empty });

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.IsTrue(actual.ValidationDictionary.ContainsKey(nameof(GetTransferAllowanceRequest.HashedAccountId)));
        }

        [Test]
        public async Task ThenTheRequestIsInvalidIfTheFieldIsNull()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetTransferAllowanceRequest());

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public async Task ThenTheRequestIsValidIfTheFieldsArePopulated()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetTransferAllowanceRequest { HashedAccountId = "1234" });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenNonAccountMembersShouldBeUnauthorised()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                                 .ReturnsAsync(null);

            //Act
            var actual = await _validator.ValidateAsync(new GetTransferAllowanceRequest { HashedAccountId = "1234" });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}
