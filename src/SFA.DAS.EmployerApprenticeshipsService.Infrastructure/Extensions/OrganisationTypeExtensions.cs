using SFA.DAS.EAS.Infrastructure.Exceptions;
using CommonOrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;
using ReferenceDataOrganisationType = SFA.DAS.ReferenceData.Types.DTO.OrganisationType;

namespace SFA.DAS.EAS.Infrastructure.Extensions
{
    public static class OrganisationTypeExtensions
    {
        public static CommonOrganisationType ToCommonOrganisationType(this ReferenceDataOrganisationType organisationType)
        {
            switch (organisationType)
            {
                case ReferenceDataOrganisationType.Charity:
                    return CommonOrganisationType.Charities;
                case ReferenceDataOrganisationType.Company:
                    return CommonOrganisationType.CompaniesHouse;
                case ReferenceDataOrganisationType.EducationOrganisation:
                    return CommonOrganisationType.PublicBodies;
                case ReferenceDataOrganisationType.PublicSector:
                    return CommonOrganisationType.PublicBodies;
                default:
                    return CommonOrganisationType.Other;
            }
        }

        public static ReferenceDataOrganisationType ToReferenceDataOrganisationType(this CommonOrganisationType organisationType)
        {
            switch (organisationType)
            {
                case CommonOrganisationType.Charities: return ReferenceDataOrganisationType.Charity;
                case CommonOrganisationType.CompaniesHouse: return ReferenceDataOrganisationType.Company;
                default:
                    // It is not possible to convert values of PublicBodies and Other to a reference data organisation type (as the mapping is 1:m). 
                    throw new InvalidOrganisationTypeConversionException($"Can not convert Reference Data {organisationType} to a value of {typeof(ReferenceDataOrganisationType).FullName}");
            }
        }
    }
}
