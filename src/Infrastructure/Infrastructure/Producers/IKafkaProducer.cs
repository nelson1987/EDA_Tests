using EDA.Api.Controllers;

namespace EDA.Infrastructure.Producers;

public interface IKafkaProducer<T> where T : IEvent
{
    Task Publish(T @event);
}
