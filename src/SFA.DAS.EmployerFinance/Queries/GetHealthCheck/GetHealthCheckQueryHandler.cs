using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Dtos;

namespace SFA.DAS.EmployerFinance.Queries.GetHealthCheck
{
    public class GetHealthCheckQueryHandler : IAsyncRequestHandler<GetHealthCheckQuery, GetHealthCheckResponse>
    {
        private readonly IConfigurationProvider _configurationProvider;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public GetHealthCheckQueryHandler(IConfigurationProvider configurationProvider, Lazy<EmployerFinanceDbContext> db)
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