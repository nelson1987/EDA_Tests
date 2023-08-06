using EDA.Domain.Entities;

namespace EDA.Api.Controllers;
public record AberturaContaCommandResponse(Guid Id, string ClientName, string ClientDocument)
{
    public static implicit operator AberturaContaCommandResponse(Conta entitiy)
    {
        return new AberturaContaCommandResponse(entitiy.Id, entitiy.Name, entitiy.Document);
    }
}
