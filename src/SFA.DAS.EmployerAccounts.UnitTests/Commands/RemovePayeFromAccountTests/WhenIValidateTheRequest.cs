using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.RemovePayeFromAccountTests
{
    public class WhenIValidateTheRequest
    {
        private RemovePayeFromAccountCommandValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;


        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { AccountId = 12345, Role = Role.Owner });

            _validator = new RemovePayeFromAccountCommandValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenItIsValidIfAllFieldsArePopulated()
        {
            //Act
            var result =
                await _validator.ValidateAsync(new RemovePayeFromAccountCommand("12345", "123RFD", "123edds", true,
                    "companyName"));


            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenTheValidationDictionaryIsPopulatedWhenThereAreErrors()
        {
            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand(null, null, null, false, null));

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsNotEmpty(result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("PayeRef","PayeRef has not been supplied"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserId","UserId has not been supplied"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("RemoveScheme", "Please confirm you wish to remove the scheme"), result.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyAreAnOwnerAndUnauthroizedIsSetOnTheResult()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {AccountId = 12345, Role = Role.Transactor});

            //Act
            var result =
                await _validator.ValidateAsync(new RemovePayeFromAccountCommand("12345", "123RFD", "123edds", true,"companyName"));


            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyArePartOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(() => null);

            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand("12345", "123RFD", "123edds", true, "companyName"));


            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }
    }
}
