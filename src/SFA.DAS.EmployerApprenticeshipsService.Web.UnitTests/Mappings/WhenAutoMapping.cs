using AutoMapper;
using NUnit.Framework;
using SFA.DAS.EAS.Web.Mappings;

namespace SFA.DAS.EAS.Web.UnitTests.Mappings
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
                c.AddProfile<TransferMappings>();
                c.AddProfile<OrganisationMappings>();
                c.AddProfile<AgreementMappings>();
            });

            config.AssertConfigurationIsValid();
        }
    }
}