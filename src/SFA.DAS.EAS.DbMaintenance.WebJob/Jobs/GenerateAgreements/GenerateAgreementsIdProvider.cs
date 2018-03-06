using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.GenerateAgreements
{
    public class GenerateAgreementsIdProvider : IIdProvider
    {
        private readonly ILegalEntityRepository _legalEntityRepository;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;

        public GenerateAgreementsIdProvider(
            ILegalEntityRepository legalEntityRepository,
            IEmployerAgreementRepository employerAgreementRepository)
        {
            _legalEntityRepository = legalEntityRepository;
            _employerAgreementRepository = employerAgreementRepository;
        }

        public async Task<IEnumerable<long>> GetIdsAsync(long startAfterId, int count, ProcessingContext processingContext)
        {
            var latestTemplate = await _employerAgreementRepository.GetLatestAgreementTemplate();

            processingContext.Set(Constants.ProcessingContextValues.LatestTemplateId, latestTemplate.Id);

            return await _legalEntityRepository.GetLegalEntitiesWithoutSpecificAgreement(
                                            startAfterId + 1, 
                                            count,
                                            latestTemplate.Id);
        }
    }
}
