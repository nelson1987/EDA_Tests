using EDA.Domain.Entities;

namespace EDA.Api.Controllers;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task Commit();
    Task RollbackAsync();
    IRepository<T> Repository<T>() where T : IEntity;
}
