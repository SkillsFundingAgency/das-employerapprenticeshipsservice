using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.Client.Services.Recruit.Models
{
    internal class VacanciesSummary
    {
        public IEnumerable<VacancySummary> Vacancies { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }

        public VacanciesSummary(IEnumerable<VacancySummary> vacancies, int pageSize, int pageNo, int totalResults, int totalPages)
        {
            Vacancies = vacancies;
            PageSize = pageSize;
            PageNo = pageNo;
            TotalResults = totalResults;
            TotalPages = totalPages;
        }
    }
}
