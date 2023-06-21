using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EAS.Support.ApplicationServices.Models;

public enum SearchCategory
{
    None,
    User,
    Account,
    Apprentice
}

public class AccountSearchModel
{
    public string Account { get; set; }
    public string AccountID { get; set; }
    public string PublicAccountID { get; set; }
    public SearchCategory SearchType { get; set; }
    public List<string> PayeSchemeIds { get; set; }
    public string AccountSearchKeyWord { get => Account.ToLower(); }
    public string AccountIDSearchKeyWord { get => AccountID.ToLower(); }
    public string PublicAccountIDSearchKeyWord { get => PublicAccountID.ToLower(); }
    public IEnumerable<string> PayeSchemeIdSearchKeyWords
    {
        get
        {
            return PayeSchemeIds?.Select(o => o.ToLower());
        }
    }
}