using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.Account;

public class EmployerAccountOrganisationData
{
    public OrganisationType OrganisationType { get; set; }
    public short? PublicSectorDataSource { get; set; }
    public string OrganisationName { get; set; }
    public string OrganisationReferenceNumber { get; set; }
    public string OrganisationRegisteredAddress { get; set; }
    public DateTime? OrganisationDateOfInception { get; set; }
    public string OrganisationStatus { get; set; }
    public string Sector { get; set; }
    public bool NewSearch { get; set; }
    public bool PensionsRegulatorReturnedMultipleResults { get; set; }
}