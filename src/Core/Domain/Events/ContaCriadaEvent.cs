using EDA.Domain.Entities;

namespace EDA.Api.Controllers;
public record ContaCriadaEvent(string ClientName, string ClientDocument) : IEvent
{
    public static implicit operator ContaCriadaEvent(Conta command)
    {
        return new ContaCriadaEvent(command.Name, command.Document);
    }
}
