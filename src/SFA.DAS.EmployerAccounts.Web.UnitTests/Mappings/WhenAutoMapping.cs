using AutoMapper;
using NUnit.Framework;

using SFA.DAS.EmployerAccounts.Mappings;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Mappings
{
    [TestFixture]
    public class WhenAutoMapping
    {
        [Test]
        public void ThenShouldUseValidConfiguration()
        {
            var config = new MapperConfiguration(c =>
            {
                c.AddProfile<ActivityMappings>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}