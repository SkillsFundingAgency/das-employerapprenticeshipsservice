using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RemovePayeFromAccount;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemovePayeFromAccountTests
{
    public class WhenIValidateTheRequest
    {
        private RemovePayeFromAccountCommandValidator _validator;
        private Mock<IMembershipRepository> _membershipRepository;


        [SetUp]
        public void Arrange()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView { AccountId = 12345, RoleId = (short)Role.Owner });

            _validator = new RemovePayeFromAccountCommandValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenItIsValidIfAllFieldsArePopulated()
        {
            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand
                {
                    HashedAccountId = "12345",
                    PayeRef = "123RFD",
                    UserId = "123edds",
                    RemoveScheme = true
            });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenTheValidationDictionaryIsPopulatedWhenThereAreErrors()
        {
            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand());

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
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MembershipView {AccountId = 12345, RoleId = (short)Role.Transactor});

            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand
            {
                HashedAccountId = "12345",
                PayeRef = "123RFD",
                UserId = "123edds",
                RemoveScheme = true
            });

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUserIsCheckedToSeeIfTheyArePartOfTheAccount()
        {
            //Arrange
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(null);

            //Act
            var result = await _validator.ValidateAsync(new RemovePayeFromAccountCommand
            {
                HashedAccountId = "12345",
                PayeRef = "123RFD",
                UserId = "123edds",
                RemoveScheme = true
            });

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }
    }
}
