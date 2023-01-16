using System.Threading;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EntityFramework;
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
        var accountsQuery = _accountDb.Value.Accounts.FutureCount();
        var legalEntitiesQuery = _accountDb.Value.LegalEntities.FutureCount();
        var payeSchemesQuery = _accountDb.Value.Payees.FutureCount();
        var agreementsQuery = _accountDb.Value.Agreements.Where(a => a.StatusId == EmployerAgreementStatus.Signed).FutureCount();

        var statistics = new Statistics
        {
            TotalAccounts = await accountsQuery.ValueAsync(),
            TotalLegalEntities = await legalEntitiesQuery.ValueAsync(),
            TotalPayeSchemes = await payeSchemesQuery.ValueAsync(),
            TotalAgreements = await agreementsQuery.ValueAsync()
        };

        return new GetStatisticsResponse
        {
            Statistics = statistics
        };
    }
}