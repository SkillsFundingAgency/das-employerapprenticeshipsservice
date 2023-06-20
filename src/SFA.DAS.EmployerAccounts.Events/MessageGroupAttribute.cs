using System;

namespace SFA.DAS.EmployerAccounts.Events;

public class MessageGroupAttribute : Attribute
{
    public MessageGroupAttribute(string name = "") { }
}