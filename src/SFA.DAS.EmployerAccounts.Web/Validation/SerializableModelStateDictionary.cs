using System;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.Validation
{
    [Serializable]
    public class SerializableModelStateDictionary
    {
        public ICollection<SerializableModelState> Data { get; set; } = new List<SerializableModelState>();
    }
}