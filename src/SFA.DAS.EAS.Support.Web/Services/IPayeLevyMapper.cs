using SFA.DAS.EAS.Support.ApplicationServices.Models;
using SFA.DAS.EAS.Support.Web.Models;

namespace SFA.DAS.EAS.Support.Web.Services;

public interface IPayeLevyMapper
{
    PayeSchemeLevyDeclarationViewModel MapPayeLevyDeclaration(PayeLevySubmissionsResponse model);
}