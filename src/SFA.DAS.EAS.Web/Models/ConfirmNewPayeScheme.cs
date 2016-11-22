using System;

namespace SFA.DAS.EAS.Web.Models
{
    public class ConfirmNewPayeScheme : AddNewPayeScheme
    {
        public ConfirmNewPayeScheme()
        {
        }

        public ConfirmNewPayeScheme(AddNewPayeScheme model)
        {
            AccessToken = model.AccessToken;
            RefreshToken = model.RefreshToken;
            PayeScheme = model.PayeScheme;
            HashedId = model.HashedId;
        }

    }
}