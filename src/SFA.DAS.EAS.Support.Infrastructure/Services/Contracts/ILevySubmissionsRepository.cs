using HMRC.ESFA.Levy.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Services.Contracts;

public interface ILevySubmissionsRepository
{
    Task<LevyDeclarations> Get(string payeScheme);
}
