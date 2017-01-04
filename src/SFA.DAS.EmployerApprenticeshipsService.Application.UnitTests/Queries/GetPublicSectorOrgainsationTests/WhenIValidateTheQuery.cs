using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetPublicSectorOrganisation;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetPublicSectorOrgainsationTests
{
    public class WhenIValidateTheQuery
    {
        private GetPublicSectorOrgainsationValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetPublicSectorOrgainsationValidator();
        }

        [Test]
        public void ThenAValidQueryShouldPassValidation()
        {
            //Arange
            var query = new GetPublicSectorOrganisationQuery
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 50
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenAQueryWithNoSearchTermShouldFailValidation()
        {
            //Arange
            var query = new GetPublicSectorOrganisationQuery
            {
                SearchTerm = null,
                PageNumber = 1,
                PageSize = 50
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("SearchTerm", "Search term has not been supplied"), result.ValidationDictionary);
        }

        [Test]
        public void ThenAQueryWithNoPageSizeShouldFailValidation()
        {
            //Arange
            var query = new GetPublicSectorOrganisationQuery
            {
                SearchTerm = "test",
                PageNumber = 1,
                PageSize = 0
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PageSize", "Page size must be greater than zero"), result.ValidationDictionary);
        }

        [Test]
        public void ThenAQueryWithNoPageNumberShouldFailValidation()
        {
            //Arange
            var query = new GetPublicSectorOrganisationQuery
            {
                SearchTerm = "test",
                PageNumber = 0,
                PageSize = 50
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("PageNumber", "Page number must be greater than zero"), result.ValidationDictionary);
        }

        [Test]
        public void ThenANullQueryShouldFailValidation()
        {
            //Act
            var result = _validator.Validate(null);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("Query", "Query should not be null"), result.ValidationDictionary);

        }
    }
}
