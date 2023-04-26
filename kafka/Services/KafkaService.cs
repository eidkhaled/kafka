using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace kafta.Services
{
    public class KafkaService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public KafkaService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var consumer = scope.ServiceProvider.GetRequiredService<IConsumer<Ignore, string>>();
                consumer.Subscribe("test-topic");

                var producer = scope.ServiceProvider.GetRequiredService<IProducer<Null, string>>();

                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(_cancellationTokenSource.Token);
                        var message = consumeResult.Message;

                        Console.WriteLine($"Received message at {DateTime.Now}: {message.Value}");

                        var producerMessage = new Message<Null, string>
                        {
                            Value = $"Echo: {message.Value}"
                        };
                        await producer.ProduceAsync("echo-topic", producerMessage, _cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing message: {ex.Message}");
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
