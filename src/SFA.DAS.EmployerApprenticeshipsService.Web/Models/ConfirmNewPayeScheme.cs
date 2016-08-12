using System;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public class ConfirmNewPayeScheme : AddNewPayeScheme
    {
        public ConfirmNewPayeScheme()
        {
        }

        public ConfirmNewPayeScheme(AddNewPayeScheme model)
        {
            AccountId = model.AccountId;
            AccessToken = model.AccessToken;
            RefreshToken = model.RefreshToken;
            PayeScheme = model.PayeScheme;
        }

        public long LegalEntityId { get; set; }

        public string LegalEntityCode { get; set; }

        public string LegalEntityName { get; set; }

        public string LegalEntityRegisteredAddress { get; set; }

        public DateTime LegalEntityDateOfIncorporation { get; set; }

    }
}