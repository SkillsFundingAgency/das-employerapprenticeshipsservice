using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using SFA.DAS.EAS.Web.Validators;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class SelectLegalEntityViewModel
    {
        [Required(ErrorMessage = "Choose organisation")]
        public string LegalEntityCode { get; set; }
    }

    [Validator(typeof(SelectProviderViewModelValidator))]
    public sealed class SelectProviderViewModel
    {
        public string LegalEntityCode { get; set; }

        public string ProviderId { get; set; }
    }

    public sealed class CreateCommitmentViewModel
    {
        // TODO: LWA No longer needed. Delete.
        public string Name { get; set; }

        [Required]
        public string HashedAccountId { get; set; }

        [Required]
        public string LegalEntityCode { get; set; }

        public string LegalEntityName { get; set; }

        [Required]
        public long ProviderId { get; set; }

        public string ProviderName { get; set; }    
    }
}