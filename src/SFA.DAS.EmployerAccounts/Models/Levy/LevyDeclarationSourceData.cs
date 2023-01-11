namespace SFA.DAS.EmployerAccounts.Models.Levy;

public class LevyDeclarationSourceData
{
    public long AccountId { get; set; }

    public List<LevyDeclarationSourceDataItem> Data { get; set; }
}