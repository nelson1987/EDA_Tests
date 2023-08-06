using EDA.Domain.Entities;

namespace EDA.Infrastructure.Services;

public interface IEmailService
{
    Task EnviarBoasVindas(Conta conta);
}
