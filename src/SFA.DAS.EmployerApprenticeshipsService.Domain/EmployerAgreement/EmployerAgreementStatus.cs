using System.ComponentModel;

namespace SFA.DAS.EAS.Domain.EmployerAgreement
{
    public enum EmployerAgreementStatus : short
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