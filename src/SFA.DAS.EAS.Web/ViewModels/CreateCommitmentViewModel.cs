using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class SelectLegalEntityViewModel
    {
        [Required(ErrorMessage = "Choose organisation")]
        public string LegalEntityCode { get; set; }

        public string CohortRef { get; set; }

        public IEnumerable<LegalEntity> LegalEntities { get; set; }
    }

    [Validator(typeof(SelectProviderViewModelValidator))]
    public sealed class SelectProviderViewModel
    {
        public string LegalEntityCode { get; set; }

        public string ProviderId { get; set; }

        public string CohortRef { get; set; }
    }

    public sealed class CreateCommitmentViewModel
    {
        public string CohortRef { get; set; }

        [Required]
        public string HashedAccountId { get; set; }

        [Required]
        public string LegalEntityCode { get; set; }

        public string LegalEntityName { get; set; }

        [Required]
        public long ProviderId { get; set; }

        public string ProviderName { get; set; }    

        [Required(ErrorMessage = "Choose an option")]
        public string SelectedRoute { get; set; }
    }
}