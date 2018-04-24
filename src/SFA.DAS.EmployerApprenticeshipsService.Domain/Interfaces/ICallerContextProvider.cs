using SFA.DAS.EAS.Domain.Models.Authorization;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ICallerContextProvider
    {
        ICallerContext GetCallerContext();
    }
}