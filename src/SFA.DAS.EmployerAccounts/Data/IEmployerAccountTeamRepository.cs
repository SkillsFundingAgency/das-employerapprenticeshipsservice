﻿namespace SFA.DAS.EmployerAccounts.Data;

public interface IEmployerAccountTeamRepository
{
    Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId);
    Task<TeamMember> GetMember(string hashedAccountId, string email, bool onlyIfMemberIsActive);
    Task<List<TeamMember>> GetAccountTeamMembers(string hashedAccountId);
}