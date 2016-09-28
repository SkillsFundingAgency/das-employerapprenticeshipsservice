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
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }    
    }
}