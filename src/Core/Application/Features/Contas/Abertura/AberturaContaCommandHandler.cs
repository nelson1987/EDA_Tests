using EDA.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EDA.Api.Controllers;
public class AberturaContaCommandHandler : IRequestHandler<AberturaContaCommand, AberturaContaCommandResponse>
{
    private readonly IContaService _contaService;
    private readonly ILogger<AberturaContaCommandHandler> _log;

    public AberturaContaCommandHandler(ILogger<AberturaContaCommandHandler> log, IContaService contaCriadaProducer)
    {
        _log = log;
        _contaService = contaCriadaProducer;
    }

    public async Task<AberturaContaCommandResponse> Handle(AberturaContaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _log.LogInformation("INI");
            Conta contaCriada = await _contaService.AbrirConta(request, cancellationToken);
            _log.LogInformation("END");
            return contaCriada;
        }
        catch (Exception exception)
        {
            _log.LogInformation("ERR", exception);
            throw;
        }
    }
}
