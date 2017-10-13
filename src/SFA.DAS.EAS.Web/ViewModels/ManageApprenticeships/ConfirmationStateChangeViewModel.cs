using System;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class ConfirmationStateChangeViewModel
    {
        public string ApprenticeName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public ChangeStatusViewModel ChangeStatusViewModel { get; set; }
    }
}