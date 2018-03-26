using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetTheRdsRequiredStatistics
{
    public class GetTheRdsRequiredStatisticsHandler : IAsyncRequestHandler<GetTheRdsRequiredStatisticsRequest, GetTheRdsRequiredStatisticsResponse>
    {
        private readonly IStatisticsRepository _repository;

        public GetTheRdsRequiredStatisticsHandler(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetTheRdsRequiredStatisticsResponse> Handle(GetTheRdsRequiredStatisticsRequest message)
        {
            var model = await _repository.GetTheRequiredRdsStatistics();
            RdsRequiredStatisticsViewModel viewModel;

            if (model == null)
            {
                viewModel = new RdsRequiredStatisticsViewModel();
            }
            else
            {
                viewModel = new RdsRequiredStatisticsViewModel
                {
                    TotalAccounts = model.TotalAccounts,
                    TotalAgreements = model.TotalAgreements,
                    TotalLegalEntities = model.TotalLegalEntities,
                    TotalPAYESchemes = model.TotalPAYESchemes,
                    TotalPayments = model.TotalPayments
                };
            }
            return new GetTheRdsRequiredStatisticsResponse
            {
                Statistics = viewModel
            };
        }
    }
}
