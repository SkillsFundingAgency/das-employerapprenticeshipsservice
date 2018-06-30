namespace SFA.DAS.EAS.Infrastructure.Authorization
{
    public interface ICallerContextProvider
    {
        ICallerContext GetCallerContext();
    }
}