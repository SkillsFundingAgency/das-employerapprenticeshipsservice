using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Models.Organisation;

namespace SFA.DAS.EmployerAccounts.Models.ReferenceData;

public class OrganisationName
{
    public string Name { get; set; }
    public OrganisationType Type { get; set; }
    public OrganisationSubType SubType { get; set; }
    public string Code { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public SFA.DAS.EmployerAccounts.Models.Organisation.Address Address { get; set; }
    public string Sector { get; set; }
    public OrganisationStatus OrganisationStatus { get; set; }
}