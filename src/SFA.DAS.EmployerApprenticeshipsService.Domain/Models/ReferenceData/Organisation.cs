using System;
using SFA.DAS.EAS.Domain.Models.Organisation;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Domain.Models.ReferenceData;

public class OrganisationName
{
    public string Name { get; set; }
    public OrganisationType Type { get; set; }
    public OrganisationSubType SubType { get; set; }
    public string Code { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public Address Address { get; set; }
    public string Sector { get; set; }
}