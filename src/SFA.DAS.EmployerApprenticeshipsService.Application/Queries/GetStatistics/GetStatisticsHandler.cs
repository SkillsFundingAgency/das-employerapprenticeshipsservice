using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Domain.Data.Repositories;
using StructureMap.Query;

namespace SFA.DAS.EAS.Application.Queries.GetStatistics
{
    public class GetStatisticsHandler : IAsyncRequestHandler<GetStatisticsRequest, GetStatisticsResponse>
    {
        private readonly IStatisticsAccountsRepository _repositoryAccounts;
        private readonly IStatisticsFinancialRepository _repositoryFinancial;

        public GetStatisticsHandler(IStatisticsAccountsRepository repositoryAccounts, IStatisticsFinancialRepository repositoryFinancial)
        {
            _repositoryAccounts = repositoryAccounts;
            _repositoryFinancial = repositoryFinancial;
        }

        public async Task<GetStatisticsResponse> Handle(GetStatisticsRequest message)
        {
            var modelAccounts = await _repositoryAccounts.GetStatistics();
            var modelFinancial = await _repositoryFinancial.GetStatistics();

            StatisticsViewModel viewModel;

            if (modelAccounts == null || modelFinancial == null )
            {
                viewModel = new StatisticsViewModel();
            }
            else
            {
                viewModel = new StatisticsViewModel
                {
                    TotalAccounts = modelAccounts.TotalAccounts,
                    TotalSignedAgreements = modelAccounts.TotalAgreements,
                    TotalActiveLegalEntities = modelAccounts.TotalLegalEntities,
                    TotalPAYESchemes = modelAccounts.TotalPAYESchemes,
                    TotalPaymentsThisYear = modelFinancial.TotalPayments
                };
            }
            return new GetStatisticsResponse
            {
                Statistics = viewModel
            };
        }
    }
}
