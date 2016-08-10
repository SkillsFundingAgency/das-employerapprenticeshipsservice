using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class ConfirmNewPayeScheme : AddNewPayeScheme
    {
        public ConfirmNewPayeScheme(AddNewPayeScheme model)
        {
            AccountId = model.AccountId;
            AccessToken = model.AccessToken;
            RefreshToken = model.RefreshToken;
            PayeScheme = model.PayeScheme;
        }
        public LegalEntity SelectedEntity { get; set; }
        
    }
}