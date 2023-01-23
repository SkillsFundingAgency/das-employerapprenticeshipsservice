using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IInvitationRepository
{
    Task<List<InvitationView>> Get(string userId);
    Task<InvitationView> GetView(long id);
    Task<long> Create(Invitation invitation);
    Task<Invitation> Get(long id);
    Task<Invitation> Get(long accountId, string email);
    Task ChangeStatus(Invitation invitation);
    Task Resend(Invitation invitation);
    Task Accept(string email, long accountId, Role role);
    Task<int> GetNumberOfInvites(string userId);
}