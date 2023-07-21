using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;

public class CreateLegalEntityCommand : IRequest<CreateLegalEntityCommandResponse>
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
    public string ExternalUserId { get; set; }
}