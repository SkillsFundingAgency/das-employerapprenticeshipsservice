using System;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public sealed class WhenToMakeChangeViewModel
    {
        public bool SkipStep { get; set; }
        public ChangeStatusViewModel ChangeStatusViewModel { get; set; }
        public DateTime StartDate { get; internal set; }
    }
}