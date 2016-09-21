namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IUserWhiteList
    {
        bool IsEmailOnWhiteList(string email);
    }
}
