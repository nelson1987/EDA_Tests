using EDA.Domain.Entities;

namespace EDA.Api.Controllers;

public interface IRepository<T> where T : IEntity
{
    Task<List<T>> GetAll();
    Task<T> Insert(T entity);
}
