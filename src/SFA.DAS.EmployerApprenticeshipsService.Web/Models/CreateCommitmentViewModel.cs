using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class ExtendedCreateCommitmentViewModel
    {
        public CreateCommitmentViewModel Commitment { get; set; }
        public List<Provider> Providers { get; set; }
        public List<LegalEntity> LegalEntities { get; set; }
    }

    public class CreateCommitmentViewModel : CreateCommitmentModelBase
    {
        
    }

    public class CreateCommitmentModel : CreateCommitmentModelBase
    {
    }

    public abstract class CreateCommitmentModelBase
    {
        public string Name { get; set; }
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public int ProviderId { get; set; }
        public string ProviderName { get; set; }
    }
}