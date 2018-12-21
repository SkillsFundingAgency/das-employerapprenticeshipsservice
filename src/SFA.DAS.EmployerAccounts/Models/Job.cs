using System;
using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models
{
    public class Job : Entity
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime Completed { get; private set; }

        public Job(string name, DateTime completed)
        {
            Name = name;
            Completed = completed;
        }

        [JsonConstructor]
        private Job()
        {
        }
    }
}