using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Data.Contracts;

public interface IMembershipRepository
{
    Task<TeamMember> Get(long accountId, string email);
    Task<TeamMember> Get(long userId, long accountId);
    Task Remove(long userId, long accountId);
    Task ChangeRole(long userId, long accountId, Role role);
    Task<MembershipView> GetCaller(string hashedAccountId, string externalUserId);
    Task<MembershipView> GetCaller(long accountId, string externalUserId);
    Task SetShowAccountWizard(string hashedAccountId, string externalUserId, bool showWizard);
}