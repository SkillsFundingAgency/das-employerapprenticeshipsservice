using System.Linq;
using SFA.DAS.CosmosDb;
using SFA.DAS.EAS.DasRecruitService.Services;
using SFA.DAS.EAS.Portal.Client.Data;
using SFA.DAS.EAS.Portal.Client.Types;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.EAS.DasRecruitService.Models;

namespace SFA.DAS.EAS.Portal.Client.Application.Queries
{
    public class GetAccountQuery
    {
        private readonly IAccountsReadOnlyRepository _accountsReadOnlyRepository;
        private readonly IDasRecruitService _dasRecruitService;

        public GetAccountQuery(IAccountsReadOnlyRepository accountsReadOnlyRepository,
            IDasRecruitService dasRecruitService)
        {
            _accountsReadOnlyRepository = accountsReadOnlyRepository;
            _dasRecruitService = dasRecruitService;
        }

        public async Task<Account> Get(long accountId, bool getRecruit, CancellationToken cancellationToken = default)
        {
            var document = await _accountsReadOnlyRepository.GetAccountDocumentById(accountId, cancellationToken);
            if (getRecruit)
            {
                var vacanciesSummary = await _dasRecruitService.GetVacanciesSummary(accountId);
                document.Account.Vacancies = vacanciesSummary.Vacancies.Select(Map).ToList();
            }

            return document?.Account;
        }

        private Vacancy Map(VacancySummary summary)
        {
            return new Vacancy
            {
                ClosingDate = summary.ClosingDate,
                ManageVacancyUrl = summary.RaaManageVacancyUrl,
                NumberOfApplications = summary.NoOfNewApplications+summary.NoOfSuccessfulApplications+summary.NoOfUnsuccessfulApplications,
                Reference = summary.VacancyReference,
                Status = StringToStatus(summary.Status),
                Title = summary.Title,
                TrainingTitle = summary.TrainingTitle
            };
        }

        private VacancyStatus StringToStatus(string summaryStatus)
        {
            var status = VacancyStatus.None;

            switch (summaryStatus)
            {
                case "Live":
                    status = VacancyStatus.Live;
                    break;
                case "Closed":
                    status = VacancyStatus.Closed;
                    break;
                case "Rejected":
                    status = VacancyStatus.Rejected;
                    break;
                case "Draft":
                    status = VacancyStatus.Draft;
                    break;
                case "PendingReview":
                    status = VacancyStatus.PendingReview;
                    break;
                default:
                    status = VacancyStatus.None;
                    break;
            }

            return status;
        }
    }
}