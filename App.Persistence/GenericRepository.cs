using System.Linq.Expressions;
using App.Application.Contracts.Persistence;
using App.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace App.Persistence;

public class GenericRepository<T, TId>(StockDbContext context) : IGenericRepository<T, TId>
    where T : BaseEntity<TId>
    where TId : struct
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public Task<List<T>> GetAllAsync()
    {
        return _dbSet.AsNoTracking().ToListAsync();
    }

    public Task<List<T>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        return _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
    }

    public IQueryable<T> Where(Expression<Func<T, bool>> predicate) => _dbSet.Where(predicate);

    // id türü artık TId
    public ValueTask<T?> GetByIdAsync(TId id) => _dbSet.FindAsync(id);
    
    public async ValueTask AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public Task<bool> AnyAsync(TId id) => _dbSet.AnyAsync(x => x.Id.Equals(id));

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.AnyAsync(predicate);
    }
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.FirstOrDefaultAsync(predicate);
    }

}