using AutoMapper;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Web.Mappings;
using AgreementMappings = SFA.DAS.EmployerAccounts.Web.Mappings.AgreementMappings;
using HealthCheckMappings = SFA.DAS.EmployerAccounts.Web.Mappings.HealthCheckMappings;
using VacancyMappings = SFA.DAS.EmployerAccounts.Web.Mappings.VacancyMappings;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Mappings;

[TestFixture]
public class WhenAutoMapping
{
    [Test]
    public void ThenShouldUseValidConfiguration()
    {
        var config = new MapperConfiguration(c =>
        {
            c.AddProfile<AgreementMappings>();
            c.AddProfile<CohortMapping>();
            c.AddProfile<HealthCheckMappings>();
            c.AddProfile<OrganisationMappings>();
            c.AddProfile<VacancyMappings>();
        });
        
        config.AssertConfigurationIsValid();
    }
}