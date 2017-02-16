using System.ComponentModel;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public enum EmployerAgreementStatus
    {
        [Description("Not signed")]
        Pending = 1,
        [Description("Signed")]
        Signed = 2,
        [Description("Expired")]
        Expired = 3,
        [Description("Superceded")]
        Superceded = 4
    }
}