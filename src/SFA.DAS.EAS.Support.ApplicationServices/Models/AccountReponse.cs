namespace SFA.DAS.EAS.Support.ApplicationServices.Models;

public class AccountReponse
{
    public Core.Models.Account Account { get; set; }

    public SearchResponseCodes StatusCode { get; set; }
}