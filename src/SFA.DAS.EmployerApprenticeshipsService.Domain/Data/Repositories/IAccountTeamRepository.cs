﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.AccountTeam;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IAccountTeamRepository
    {
        Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, Guid externalUserId);
        Task<TeamMember> GetMember(string hashedAccountId, string email);
        Task<ICollection<TeamMember>> GetAccountTeamMembers(string hashedAccountId);
    }
}
