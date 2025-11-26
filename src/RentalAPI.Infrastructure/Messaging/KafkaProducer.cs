using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using RentalAPI.Application.Interfaces;

namespace RentalAPI.Infrastructure.Messaging;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducer(IConfiguration configuration)
    {
        var bootstrapServers = configuration.GetSection("KafkaSettings:BootstrapServers").Value 
            ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = message
            });

            Console.WriteLine($"Mensagem enviada para {result.Topic}, partition {result.Partition}, offset {result.Offset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"Erro ao enviar mensagem: {ex.Error.Reason}");
        }
    }
}
