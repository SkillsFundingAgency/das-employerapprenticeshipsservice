using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountsByHashedId;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountsByHashedIdTests
{
    public class WhenIValidateTheQuery
    {
        private GetEmployerAccountsByHashedIdValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetEmployerAccountsByHashedIdValidator();
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheHashedIdIsntPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAccountsByHashedIdQuery());

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public void ThenTrueIsReturnedWhenTheHashedIdHasBeenPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetEmployerAccountsByHashedIdQuery
            {
                HashedAccountId = "ABC123"
            });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }
    }
}
