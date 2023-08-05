using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EDA.Api.Controllers;
[ApiController]
[Route("api/[controller]s")]
public class ContaController : ControllerBase
{
    private readonly ILogger<ContaController> _logger;
    private readonly IMediator _mediator;

    public ContaController(ILogger<ContaController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AberturaContaCommand command, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            //https://masstransit.io/documentation/configuration/transports/kafka
            //https://www.macoratti.net/21/01/c_cancelasync1.htm
            //https://www.infoworld.com/article/3692811/how-to-use-the-unit-of-work-pattern-in-aspnet-core.html
            //https://dotnettutorials.net/lesson/unit-of-work-csharp-mvc/
            AberturaContaCommandResponse response = await _mediator.Send(command, cancellationToken);
            return Ok(response);

        }
        return Problem();
    }
}
public record AberturaContaCommand : IRequest<AberturaContaCommandResponse>
{
    public string ClientName { get; set; }
    public string ClientDocument { get; set; }
    public static implicit operator Conta(AberturaContaCommand command)
    {
        return new Conta(command.ClientName, command.ClientDocument);
    }
    public static implicit operator ContaCriadaEvent(AberturaContaCommand command)
    {
        return new ContaCriadaEvent(command.ClientName, command.ClientDocument);
    }
}
public record AberturaContaCommandResponse(string ClientName, string ClientDocument);
public record ContaCriadaEvent(string ClientName, string ClientDocument) : IEvent;
public record Conta(string ClientName, string ClientDocument) : IEntity;
public class AberturaContaCommandHandler : IRequestHandler<AberturaContaCommand, AberturaContaCommandResponse>
{
    private readonly ILogger<AberturaContaCommandHandler> _log;
    private readonly IKafkaProducer<ContaCriadaEvent> _contaCriadaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpLogisticaService _httpLogisticaService;

    public AberturaContaCommandHandler(ILogger<AberturaContaCommandHandler> log, IKafkaProducer<ContaCriadaEvent> contaCriadaProducer, IUnitOfWork unitOfWork, IHttpLogisticaService httpLogisticaService)
    {
        _log = log;
        _contaCriadaProducer = contaCriadaProducer;
        _unitOfWork = unitOfWork;
        _httpLogisticaService = httpLogisticaService;
    }

    public async Task<AberturaContaCommandResponse> Handle(AberturaContaCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _log.LogInformation("INI");
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Conta>().Insert(request);
            await _unitOfWork.Commit();
            await _contaCriadaProducer.Publish(request);
            await _httpLogisticaService.EnviarCartaoDebito(request);
            _log.LogInformation("END");
        }
        catch (Exception exception)
        {
            await _unitOfWork.RollbackAsync();
            _log.LogInformation("ERR", exception);
        }
        return new AberturaContaCommandResponse(request.ClientName, request.ClientDocument);
    }
}
public interface IEntity
{

}
public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task Commit();
    Task RollbackAsync();
    IRepository<T> Repository<T>() where T : IEntity;
}
public interface IRepository<T> where T : IEntity
{
    Task<List<T>> GetAll();
    Task<T> Insert(T entity);
}
public interface IEvent
{

}
public interface IKafkaProducer<T> where T : IEvent
{
    Task Publish(T @event);
}
public interface IHttpLogisticaService
{
    Task EnviarCartaoDebito(Conta conta);
}