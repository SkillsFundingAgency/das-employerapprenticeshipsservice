using SFA.DAS.EAS.Domain.Models.Account;

namespace SFA.DAS.EAS.Application.Queries.GetPayeSchemeByRef
{
    /// <summary>
    ///  AML-2454: Copy to finance
    /// </summary>
    public class GetPayeSchemeByRefResponse
    {
        public PayeSchemeView PayeScheme { get; set; }
    }
}