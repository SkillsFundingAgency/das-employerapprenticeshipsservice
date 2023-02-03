using System.Threading;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Api.Types;
using EmployerAgreementStatus = SFA.DAS.EmployerAccounts.Models.EmployerAgreement.EmployerAgreementStatus;

namespace SFA.DAS.EmployerAccounts.Queries.GetStatistics;

public class GetStatisticsQueryHandler : IRequestHandler<GetStatisticsQuery, GetStatisticsResponse>
{
    private readonly Lazy<EmployerAccountsDbContext> _accountDb;

    public GetStatisticsQueryHandler(Lazy<EmployerAccountsDbContext> accountDb)
    {
        _accountDb = accountDb;
    }

    public async Task<GetStatisticsResponse> Handle(GetStatisticsQuery message, CancellationToken cancellationToken)
    {
        var accountsCount = await _accountDb.Value.Accounts.CountAsync(cancellationToken);
        var legalEntitiesCount = await _accountDb.Value.LegalEntities.CountAsync(cancellationToken);
        var payeSchemesCount = await _accountDb.Value.Payees.CountAsync(cancellationToken);
        var agreementsCount = await _accountDb.Value.Agreements.Where(a => a.StatusId == EmployerAgreementStatus.Signed)
                                                                         .CountAsync(cancellationToken);

        var statistics = new Statistics
        {
            TotalAccounts = accountsCount,
            TotalLegalEntities = legalEntitiesCount,
            TotalPayeSchemes = payeSchemesCount,
            TotalAgreements = agreementsCount
        };

        return new GetStatisticsResponse
        {
            Statistics = statistics
        };
    }
}