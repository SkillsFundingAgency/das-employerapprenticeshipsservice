using System.Collections.Generic;
using Moq;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.Events.Api.Client;

namespace SFA.DAS.EAS.TestCommon.Steps
{
    public class ObjectContext
    {
        public Dictionary<string, Account> Accounts = new Dictionary<string, Account>();
        public Dictionary<string, User> Users = new Dictionary<string, User>();
        public Dictionary<string, PayeScheme> PayeSchemes = new Dictionary<string, PayeScheme>();
        public Dictionary<string, LegalEntity> LegalEntities = new Dictionary<string, LegalEntity>();
        public List<Membership> Memberships = new List<Membership>();
        public Mock<IEventsApi> EventsApiMock = new Mock<IEventsApi>();
    }
}