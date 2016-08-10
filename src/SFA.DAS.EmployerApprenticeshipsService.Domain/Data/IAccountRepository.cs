using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IAccountRepository
    {
        Task<long> CreateAccount(long userId, string employerNumber, string employerName, string employerRegisteredAddress, DateTime employerDateOfIncorporation, string employerRef, string accessToken, string refreshToken);
        Task<List<PayeView>> GetPayeSchemes(long accountId);
        Task AddPayeToAccountForExistingLegalEntity(long accountId, long legalEntityId, string employerRef, string accessToken, string refreshToken);
    }
}