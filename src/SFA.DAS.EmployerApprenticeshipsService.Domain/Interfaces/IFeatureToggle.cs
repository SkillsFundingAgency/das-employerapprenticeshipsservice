using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.FeatureToggle;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IFeatureToggle
    {
        FeatureToggleLookup GetFeatures();
    }
}
