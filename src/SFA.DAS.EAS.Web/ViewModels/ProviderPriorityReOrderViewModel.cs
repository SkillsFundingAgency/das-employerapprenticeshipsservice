using FluentValidation.Attributes;
using SFA.DAS.EAS.Web.Validators;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels
{
    [Validator(typeof(ProviderPriorityReOrderListValidator))]
    public sealed class ProviderPriorityReOrderViewModel
    {
        public ProviderPriorityReOrderViewModel()
        {
            Priorities = new List<long>();
        }

        public IList<long> Priorities { get; set; }
    }
}