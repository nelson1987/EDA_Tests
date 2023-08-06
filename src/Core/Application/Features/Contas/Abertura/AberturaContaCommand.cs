using EDA.Domain.Entities;
using MediatR;

namespace EDA.Api.Controllers;
public record AberturaContaCommand : IRequest<AberturaContaCommandResponse>
{
    public static implicit operator Conta(AberturaContaCommand command)
    {
        return new Conta(command.ClientName, command.ClientDocument);
    }

    public string ClientDocument { get; set; }
    public string ClientName { get; set; }
}
