using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.Serialization
{
    [Serializable]
    public class SerializableModelStateDictionary
    {
        public ICollection<SerializableModelState> Data { get; set; } = new List<SerializableModelState>();
    }
}