using EDA.Domain.Entities;

namespace EDA.Infrastructure.HttpServices;

public interface IHttpLogisticaService
{
    Task EnviarCartaoDebito(Conta conta);
}
