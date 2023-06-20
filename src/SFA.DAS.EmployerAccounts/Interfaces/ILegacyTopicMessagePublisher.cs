namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface ILegacyTopicMessagePublisher
{
    Task PublishAsync<T>(T message);
}