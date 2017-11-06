

namespace SFA.DAS.EmployerAccounts.Events.Messages
{
    public class PersonMessage : Message
    {
        //public PersonMessage()
        //{
        //}

        public PersonMessage(string signedByName)
        {
            SignedByName = signedByName;
        }

        public string SignedByName { get; }
    }
}
