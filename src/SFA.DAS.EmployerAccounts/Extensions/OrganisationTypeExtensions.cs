using SFA.DAS.EmployerAccounts.Exceptions;
using CommonOrganisationType = SFA.DAS.Common.Domain.Types.OrganisationType;
using ReferenceDataOrganisationType = SFA.DAS.ReferenceData.Types.DTO.OrganisationType;

namespace SFA.DAS.EmployerAccounts.Extensions;

/// <remarks>
///     There are two organisation types used within MA - one from ReferenceData and one from common types (there is also a third from commitments but this is not relevant here).
///     The two organisation types look similar but are not identical and are subtly incompatible. I suspect this was by accident but now it is running free in the wild it is a bit
///     more difficult to fix. 
///     There has always been an implicit mapping that occurs when the json from reference data is de-serialised in MA. This mapping was hitherto obscured but a recent update to 
///     reference data that added the organisation type to the API has made the mapping a lot more obvious. 
///     This class provides extensions methods to encapsulate this mapping. 
/// </remarks>
public static class OrganisationTypeExtensions
{
    /// <summary>
    ///     Map the supplied reference data organisation type to a common organisation type. 
    ///     This is always possible but both education and public bodies in the reference data world map to public bodies 
    ///     in the common types world. So once we've mapped one of these values from ref data we can not map them back.
    /// </summary>
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

    /// <summary>
    ///     Maps the supplied common organisation type to a reference data organisation type and throws an exception if this cannot be done.
    /// </summary>
    public static ReferenceDataOrganisationType ToReferenceDataOrganisationType(this CommonOrganisationType organisationType)
    {
        if (organisationType.TryToReferenceDataOrganisationType(out ReferenceDataOrganisationType result))
        {
            return result;
        }

        // It is not possible to convert values of PublicBodies and Other to a reference data organisation type (as the mapping is 1:m). 
        throw new InvalidOrganisationTypeConversionException($"Can not convert Reference Data {organisationType} to a value of {typeof(ReferenceDataOrganisationType).FullName}");
    }

    /// <summary>
    ///     Maps the supplied common organisation type to a reference data organisation type and returns false if this cannot be done.
    /// </summary>
    public static bool TryToReferenceDataOrganisationType(this CommonOrganisationType organisationType, out ReferenceDataOrganisationType referenceDataType)
    {
        switch (organisationType)
        {
            case CommonOrganisationType.Charities:
                referenceDataType = ReferenceDataOrganisationType.Charity;
                return true;

            case CommonOrganisationType.CompaniesHouse:
                referenceDataType = ReferenceDataOrganisationType.Company;
                return true;

            default:
                // We have to set the out param to something and there is no 'unknown'
                referenceDataType = ReferenceDataOrganisationType.Charity;
                return false;
        }
    }

    public static string GetFriendlyName(this CommonOrganisationType commonOrganisationType)
    {
        switch (commonOrganisationType)
        {
            case CommonOrganisationType.CompaniesHouse: return "Companies House";
            case CommonOrganisationType.Charities: return "Charity Commission";
            case CommonOrganisationType.PublicBodies: return "Public Bodies";
            case CommonOrganisationType.PensionsRegulator: return "The Pensions Regulator";
            default: return "Other";
        }
    }
}