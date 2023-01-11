using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;

public class CreateLegalEntityCommand : IAsyncRequest<CreateLegalEntityCommandResponse>
{
    public string HashedAccountId { get; set; }
    public string Code { get; set; }
    public DateTime? DateOfIncorporation { get; set; }
    public byte? PublicSectorDataSource { get; set; }
    public string Sector { get; set; }
    public OrganisationType Source { get; set; }
    public string Status { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }

    //TODO: the two signed fields don't appear to be used?
    public bool SignAgreement { get; set; }

    public DateTime SignedDate { get; set; }

    public string ExternalUserId { get; set; }
}