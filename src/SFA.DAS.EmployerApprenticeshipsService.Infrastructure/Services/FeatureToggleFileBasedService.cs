using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Services
{
    public class FeatureToggleFileBasedService : FileSystemRepository, IFeatureToggle
    {
        public FeatureToggleFileBasedService() : base("Features")
        {
        }

        public FeatureToggleLookup GetFeatures()
        {
            return ReadFileByIdNotAsync<FeatureToggleLookup>("features_data");
        }
    }
}
