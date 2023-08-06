using EDA.Domain.Entities;

namespace EDA.Api.Controllers;
public interface IContaService
{
    Task<Conta> AbrirConta(Conta conta, CancellationToken cancellationToken);
}
