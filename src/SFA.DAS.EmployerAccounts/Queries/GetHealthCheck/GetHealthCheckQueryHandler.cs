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
    public class GetHealthCheckQueryHandler : IAsyncRequestHandler<GetHealthCheckQuery, GetHealthCheckResponse>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public GetHealthCheckQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerAccountsDbContext> db)
        {
            _configurationProvider = configurationProvider;
            _db = db;
        }

        public async Task<GetHealthCheckResponse> Handle(GetHealthCheckQuery message)
        {
            var healthCheck = await _db.Value.HealthChecks
                .OrderByDescending(h => h.Id)
                .ProjectTo<HealthCheckDto>(_configurationProvider)
                .FirstOrDefaultAsync();

            return new GetHealthCheckResponse
            {
                HealthCheck = healthCheck
            };
        }
    }
}