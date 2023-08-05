using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EDA.Application.Features.Pedido.Inclusao;
public class InclusaoPedidoCommand : IRequest<InclusaoPedidoResponse>
{
    public string Produto { get; set; }
    public static implicit operator Pedido(InclusaoPedidoCommand query)
    {
        return new Pedido(query.Produto);
    }
}
public class InclusaoPedidoResponse
{
    public int Id { get; set; }
    public string Produto { get; set; }
    public int Status { get; set; }

    public static implicit operator InclusaoPedidoResponse(Pedido pedido)
    {
        return new InclusaoPedidoResponse()
        {
            Id = pedido.Id,
            Produto = pedido.Produto,
            Status = pedido.Status
        };
    }
}
public class InclusaoPedidoHandler : IRequestHandler<InclusaoPedidoCommand, InclusaoPedidoResponse>
{
    private readonly ILogger<InclusaoPedidoHandler> _logger;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IUnitOfWork _unitOfWork;

    public InclusaoPedidoHandler(ILogger<InclusaoPedidoHandler> logger, IEventDispatcher dispatcher, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _eventDispatcher = dispatcher;
        _unitOfWork = unitOfWork;
    }

    public async Task<InclusaoPedidoResponse> Handle(InclusaoPedidoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("INI", request.ToString());
        try
        {
            Pedido pedido = request;
            await _unitOfWork.Begin(cancellationToken);
            await _unitOfWork.Pedido.Incluir(pedido, cancellationToken);
            await _eventDispatcher.DispatchAsync(cancellationToken, pedido);
            await _unitOfWork.Commit(cancellationToken);
            _logger.LogInformation("END", pedido.ToString());
            return pedido;
        }
        catch (Exception exception)
        {
            await _unitOfWork.Rollback(cancellationToken);
            _logger.LogError(exception, "ERR", request.ToString());
            throw;
        }
    }
}
public class EntregaSolicitadaEvent : INotification
{
    public int PedidoId { get; set; }
}
public class EntregaSolicitadaHandler : INotificationHandler<EntregaSolicitadaEvent>
{
    private readonly ILogger<EntregaSolicitadaHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EntregaSolicitadaHandler(ILogger<EntregaSolicitadaHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(EntregaSolicitadaEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("INI", notification.ToString());
        try
        {
            await _unitOfWork.Begin(cancellationToken);
            Pedido pedido = await _unitOfWork.Pedido.BuscarPorId(notification.PedidoId, cancellationToken); ;
            await _unitOfWork.Pedido.SolicitarEntrega(pedido, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);
            _logger.LogInformation("END", pedido.ToString());
        }
        catch (Exception exception)
        {
            await _unitOfWork.Rollback(cancellationToken);
            _logger.LogError(exception, "ERR", notification.ToString());
            throw;
        }
    }
}
public class Pedido : IEntity
{
    public Pedido(string produto)
    {
        Produto = produto;
    }

    public int Id { get; private set; }
    public string Produto { get; private set; }
    public int Status { get; private set; }
}
public interface IEntity
{
}
public interface IUnitOfWork
{
    Task Begin(CancellationToken cancellationToken);
    Task Commit(CancellationToken cancellationToken);
    Task Rollback(CancellationToken cancellationToken);
    IPedidoService Pedido { get; }
}
public interface IPedidoService
{
    Task Incluir(Pedido pedido, CancellationToken cancellationToken);
    Task SolicitarEntrega(Pedido pedido, CancellationToken cancellationToken);
    Task<Pedido> BuscarPorId(int pedidoId, CancellationToken cancellationToken);
}
public interface IEventDispatcher
{
    Task DispatchAsync(CancellationToken cancellationToken, params IEntity[] entities);
}
public static class UtilExtensions
{
    public static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj);
    }
}
