using System;
using System.Collections.Generic;
//using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EmployerAccounts.Models.Recruit
{
    public class VacanciesSummary
    {
        public IEnumerable<Vacancy> Vacancies { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int TotalResults { get; set; }
        public int TotalPages { get; set; }

        public VacanciesSummary(IEnumerable<Vacancy> vacancies, int pageSize, int pageNo, int totalResults, int totalPages)
        {
            Vacancies = vacancies;
            PageSize = pageSize;
            PageNo = pageNo;
            TotalResults = totalResults;
            TotalPages = totalPages;
        }
    }

    public class Vacancy
    {
        public string Title { get; set; }
        public long? VacancyReference { get; set; }
        public string Status { get; set; }
        public DateTime? ClosingDate { get; set; }
        public string TrainingTitle { get; set; }
        public long LegalEntityId { get; set; }
    }
}