using System.Collections.Generic;

namespace SFA.DAS.EAS.Support.Web.Models;

public class PayeSchemeLevyDeclarationViewModel
{
    public string PayeSchemeName { get; set; }
    public string PayeSchemeFormatedAddedDate { get; set; }
    public string PayeSchemeRef { get; set; }
    public List<DeclarationViewModel> LevyDeclarations { get; set; }
    public bool UnexpectedError { get; set; }
}