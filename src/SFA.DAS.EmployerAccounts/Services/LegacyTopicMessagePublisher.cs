using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Services;

public class LegacyTopicMessagePublisher : ILegacyTopicMessagePublisher
{
    private readonly ITopicClientFactory _topicClientFactory;
    private readonly ILogger<LegacyTopicMessagePublisher> _logger;
    private readonly string _connectionString;

    public LegacyTopicMessagePublisher(ITopicClientFactory topicClientFactory,
        ILogger<LegacyTopicMessagePublisher> logger, string connectionString)
    {
        _topicClientFactory = topicClientFactory;
        _logger = logger;
        _connectionString = connectionString;
    }

    public async Task PublishAsync<T>(T @event)
    {
        var messageGroupName = GetMessageGroupName(@event);
        ITopicClient client = null;
        
        try
        {
            client = _topicClientFactory.CreateClient(_connectionString, messageGroupName);
            var messageBody = Serialize(@event);
            var message = new Message(messageBody);
            await client.SendAsync(message);

            _logger.LogInformation("Sent Message {TypeName} to Azure ServiceBus.", typeof(T).Name);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending Message {TypeName} to Azure ServiceBus.", typeof(T).Name);
            throw;
        }
        finally
        {
            if (client != null && !client.IsClosedOrClosing)
            {
                _logger.LogDebug("Closing legacy topic message publisher");
                await client.CloseAsync();
            }
        }
    }

    private static string GetMessageGroupName(object obj)
    {
        var customAttributeData = obj.GetType().CustomAttributes
            .FirstOrDefault((Func<CustomAttributeData, bool>)(att => att.AttributeType.Name == "MessageGroupAttribute"));
        
        var value = (string)customAttributeData?.ConstructorArguments.FirstOrDefault().Value;
       
        return !string.IsNullOrEmpty(value) ? value : obj.GetType().Name;
    }

    private static byte[] Serialize<T>(T obj)
    {
        var serializer = new DataContractSerializer(typeof(T));
        var stream = new MemoryStream();
        using (var writer = XmlDictionaryWriter.CreateBinaryWriter(stream))
        {
            serializer.WriteObject(writer, obj);
        }

        return stream.ToArray();
    }
}