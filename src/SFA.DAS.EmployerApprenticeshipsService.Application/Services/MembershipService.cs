using System;
using System.Linq;
using SFA.DAS.EAS.Application.Data;

namespace SFA.DAS.EAS.Application.Services
{
    public class MembershipService : IMembershipService
    {
        private readonly EmployerAccountDbContext _db;

        public MembershipService(EmployerAccountDbContext db)
        {
            _db = db;
        }

        public void ValidateAccountMembership(string accountHashedId, Guid userExternalId)
        {
            var isValid = _db.SqlQuery<bool>("[employer_account].[ValidateAccountMembership] @accountHashedId = {0}, @userExternalId = {1}", accountHashedId, userExternalId).Single();

            if (!isValid)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}