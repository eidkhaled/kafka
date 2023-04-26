using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
namespace kafka
{ 
public class KafkaProducer
{
    private readonly IConfiguration _configuration;
    private readonly ProducerConfig _producerConfig;
    private readonly string _topic;

    public KafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;

        _producerConfig = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = _configuration["Kafka:Username"],
            SaslPassword = _configuration["Kafka:Password"],
            SslCaLocation = _configuration["Kafka:CaCertLocation"],
        };

        _topic = _configuration["Kafka:Topic"];
    }

    public async Task ProduceAsync(string messageValue)
    {
        using var producer = new ProducerBuilder<Null, string>(_producerConfig).Build();

        var message = new Message<Null, string>
        {
            Value = messageValue,
        };

        await producer.ProduceAsync(_topic, message);
    }
} 
}