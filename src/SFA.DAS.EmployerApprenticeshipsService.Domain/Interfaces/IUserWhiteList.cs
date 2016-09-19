using System.Collections.Generic;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.WhileList;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IUserWhiteList
    {
        UserWhiteListLookUp GetList();
    }
}
