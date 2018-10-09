using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;

namespace SFA.DAS.EmployerAccounts.Queries.GetHealthCheck
{
    public class GetHealthCheckQueryHandler : IAsyncRequestHandler<GetHealthCheckQuery, GetHealthCheckQueryResponse>
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;
        private readonly IConfigurationProvider _configurationProvider;

        public GetHealthCheckQueryHandler(Lazy<EmployerAccountsDbContext> db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<GetHealthCheckQueryResponse> Handle(GetHealthCheckQuery message)
        {
            var healthCheck = await _db.Value.HealthChecks
                .OrderByDescending(h => h.Id)
                .ProjectTo<HealthCheckDto>(_configurationProvider)
                .FirstOrDefaultAsync();

            return new GetHealthCheckQueryResponse
            {
                HealthCheck = healthCheck
            };
        }
    }
}