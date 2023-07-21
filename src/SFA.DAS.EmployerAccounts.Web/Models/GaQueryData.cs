namespace SFA.DAS.EmployerAccounts.Web.Models;

public class GaQueryData
{
    [FromQuery(Name = "_ga")]
    public string _ga { get; set; }
    [FromQuery(Name = "_gl")]
    public string _gl { get; set; }
    [FromQuery(Name = "utm_source")]
    public string utm_source { get; set; }
    [FromQuery(Name = "utm_campaign")]
    public string utm_campaign { get; set; }
    [FromQuery(Name = "utm_medium")]
    public string utm_medium { get; set; }
    [FromQuery(Name = "utm_keywords")]
    public string utm_keywords { get; set; }
    [FromQuery(Name = "utm_content")]
    public string utm_content { get; set; }
}