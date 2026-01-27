using FBS.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace FBS.Infrastructure.Persistence.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    private IDbContextTransaction? _currentTransaction;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_currentTransaction is not null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            await (_currentTransaction?.CommitAsync(cancellationToken) ?? Task.CompletedTask);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await (_currentTransaction?.RollbackAsync(cancellationToken) ?? Task.CompletedTask);
        }
        finally
        {
            if (_currentTransaction is not null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
