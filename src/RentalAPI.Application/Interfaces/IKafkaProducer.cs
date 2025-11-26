namespace RentalAPI.Application.Interfaces;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string message);
}
