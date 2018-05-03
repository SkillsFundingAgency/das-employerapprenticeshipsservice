using System.Collections.Generic;
using System.Linq;
using SFA.DAS.EAS.Application.Dtos;
using SFA.DAS.EAS.Application.Dtos.EmployerAgreement;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsResponse
    {
        public List<EmployerAgreementStatusDto> EmployerAgreements { get; set; }

        public bool HasPendingAgreements =>
            EmployerAgreements != null && EmployerAgreements.Any(ag => ag.HasPendingAgreement);

        public EmployerAgreementStatusDto TryGetSinglePendingAgreement()
        {
            var onlyPendingAgreement = EmployerAgreements?.Where(x => x.HasPendingAgreement)
                .Take(2)
                .ToArray();

            if (onlyPendingAgreement?.Length == 1)
            {
                return onlyPendingAgreement[0];
            }

            return null;
        }
    }
}