using EDA.Domain.Entities;
using EDA.Infrastructure.HttpServices;
using EDA.Infrastructure.Producers;
using EDA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace EDA.Api.Controllers;

public class ContaService : IContaService
{
    private readonly ILogger<ContaService> _log;
    private readonly IKafkaProducer<ContaCriadaEvent> _contaCriadaProducer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpLogisticaService _httpLogisticaService;
    private readonly IEmailService _emailService;

    public ContaService(ILogger<ContaService> log, IKafkaProducer<ContaCriadaEvent> contaCriadaProducer, IUnitOfWork unitOfWork, IHttpLogisticaService httpLogisticaService, IEmailService emailService)
    {
        _log = log;
        _contaCriadaProducer = contaCriadaProducer;
        _unitOfWork = unitOfWork;
        _httpLogisticaService = httpLogisticaService;
        _emailService = emailService;
    }

    public async Task<Conta> AbrirConta(Conta conta, CancellationToken cancellationToken)
    {
        try
        {
            _log.LogInformation("INI");
            await _unitOfWork.BeginTransactionAsync();
            await _unitOfWork.Repository<Conta>().Insert(conta);
            await _unitOfWork.Commit();
            await _contaCriadaProducer.Publish(conta);
            await _httpLogisticaService.EnviarCartaoDebito(conta);
            await _emailService.EnviarBoasVindas(conta);
            _log.LogInformation("END");
        }
        catch (Exception exception)
        {
            await _unitOfWork.RollbackAsync();
            _log.LogInformation("ERR", exception);
        }
        return conta;
    }
}
