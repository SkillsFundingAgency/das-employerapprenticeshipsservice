using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOrganisationRemove
{
    public class GetAccountOrganisationRemoveResponse
    {
        public string Name { get; set; }
        public bool CanBeRemoved { get; set; }
        public bool HasSignedAgreement { get; set; }
    }
}