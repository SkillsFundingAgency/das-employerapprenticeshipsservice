using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos
{
    public class EmployerAccountInput
    {
        public Func<long> UserId { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public short? PublicSectorDataSource { get; set; }
        public Func<string> OrganisationName { get; set; }
        public Func<string> OrganisationReferenceNumber { get; set; }
        public string OrganisationRegisteredAddress { get; set; }
        public DateTime? OrganisationDateOfInception { get; set; }
        public Func<string> PayeReference { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public bool EmpRefNotFound { get; set; }
        public Func<string> OrganisationStatus { get; set; }
        public string EmployerRefName { get; set; }
        public Func<string> Sector { get; set; }
        public bool NewSearch { get; set; }
        public short Source { get; set; }
        public Func<string> Aorn { get; set; }
    }
}