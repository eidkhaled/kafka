using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Data;
namespace kafka
{
    public class KafkaConsumer
    {
        private readonly IConfiguration _configuration;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;

        public KafkaConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = _configuration["Kafka:Username"],
                SaslPassword = _configuration["Kafka:Password"],
                SslCaLocation = _configuration["Kafka:CaCertLocation"],
                GroupId = _configuration["Kafka:ConsumerGroupId"],
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            _topic = _configuration["Kafka:Topic"];
        }

        public void Consume(Action<string> messageHandler)
        {
            using var consumer = new ConsumerBuilder<Null, string>(_consumerConfig).Build();

            consumer.Subscribe(_topic);

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (true)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cts.Token);

                        messageHandler(consumeResult.Message.Value);
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                consumer.Close();
            }
        }
    }

}