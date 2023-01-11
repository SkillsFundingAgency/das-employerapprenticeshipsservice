using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public enum TriageOptions
{
    Yes,
    No,
    Unknown
}

public class TriageViewModel
{
    [Required]
    public TriageOptions? TriageOption { get; set; }
}