namespace SFA.DAS.EmployerAccounts.Web.Models;

public class GaQueryData
{
    [FromQuery(Name = "_ga")]
    public string Ga { get; set; }
    [FromQuery(Name = "_gl")]
    public string Gl { get; set; }
    [FromQuery(Name = "utm_source")]
    public string UtmSource { get; set; }
    [FromQuery(Name = "utm_campaign")]
    public string UtmCampaign { get; set; }
    [FromQuery(Name = "utm_medium")]
    public string UtmMedium { get; set; }
    [FromQuery(Name = "utm_keywords")]
    public string UtmKeywords { get; set; }
    [FromQuery(Name = "utm_content")]
    public string UtmContent { get; set; }
}