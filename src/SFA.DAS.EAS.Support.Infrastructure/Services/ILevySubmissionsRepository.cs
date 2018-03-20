using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;

namespace SFA.DAS.EAS.Support.Infrastructure.Services
{
    public interface ILevySubmissionsRepository
    {
        Task<LevyDeclarations> Get(string payeScheme);
    }
}
