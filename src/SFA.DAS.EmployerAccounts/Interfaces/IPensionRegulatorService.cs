using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IPensionRegulatorService
    {
        Task<IEnumerable<Organisation>> GetOrgansiationsByPayeRef(string payeRef);

        Task<IEnumerable<Organisation>> GetOrgansiationsByAorn(string aorn, string payeRef);
    }
}
