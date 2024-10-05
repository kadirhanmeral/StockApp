using System.Linq.Expressions;

namespace App.Application.Contracts.Persistence;

public interface IGenericRepository<T, TId> where T : class where TId : struct
{
    Task<List<T>> GetAllAsync();
    Task<List<T>> GetAllPagedAsync(int pageNumber, int pageSize);
    IQueryable<T> Where(Expression<Func<T, bool>> predicate);

    // id türü artık TId
    ValueTask<T?> GetByIdAsync(TId id);
    ValueTask AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);

    Task<bool> AnyAsync(TId id);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);


}