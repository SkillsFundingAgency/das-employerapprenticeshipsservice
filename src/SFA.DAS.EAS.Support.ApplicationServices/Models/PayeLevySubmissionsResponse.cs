using HMRC.ESFA.Levy.Api.Types;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Support.Core.Models;
using System;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models
{
    public class PayeLevySubmissionsResponse
    {
        public PayeSchemeModel PayeScheme { get; set; }
        public PayeLevySubmissionsResponseCodes StatusCode { get; set; }
        public LevyDeclarations LevySubmissions { get; set; }
    }
}
