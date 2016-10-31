namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IUserWhiteList
    {
        bool IsEmailOnWhiteList(string email);
    }
}
