using SFA.DAS.EmployerAccounts.Models.PensionRegulator;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IPensionRegulatorService
{
    Task<Organisation> GetOrganisationById(string organisationId);
    Task<IEnumerable<Organisation>> GetOrganisationsByPayeRef(string payeRef);
    Task<IEnumerable<Organisation>> GetOrganisationsByAorn(string aorn, string payeRef);
}