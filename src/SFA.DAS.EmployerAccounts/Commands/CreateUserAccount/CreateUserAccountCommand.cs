namespace SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;

public class CreateUserAccountCommand : IRequest<CreateUserAccountCommandResponse>
{
    public string ExternalUserId { get; set; }
    public string OrganisationName { get; set; }
}