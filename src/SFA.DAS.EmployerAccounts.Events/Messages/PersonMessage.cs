

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class PersonMessage : Message
    {
        //public PersonMessage()
        //{
        //}

        public PersonMessage(string signedByName)
        {
            signedByName = signedByName;
        }

        public string signedByName { get; }
    }
}
