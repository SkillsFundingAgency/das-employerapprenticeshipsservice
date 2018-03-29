using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetStatisticsHandler : IAsyncRequestHandler<GetStatisticsRequest, GetStatisticsResponse>
    {
        private readonly IStatisticsRepository _repository;

        public GetStatisticsHandler(IStatisticsRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsRequest message)
        {
            var model = await _repository.GetStatistics();
            StatisticsViewModel viewModel;

            if (model == null)
            {
                viewModel = new StatisticsViewModel();
            }
            else
            {
                viewModel = new StatisticsViewModel
                {
                    TotalAccounts = model.TotalAccounts,
                    TotalAgreements = model.TotalAgreements,
                    TotalLegalEntities = model.TotalLegalEntities,
                    TotalPAYESchemes = model.TotalPAYESchemes,
                    TotalPayments = model.TotalPayments
                };
            }
            return new GetStatisticsResponse
            {
                Statistics = viewModel
            };
        }
    }
}
