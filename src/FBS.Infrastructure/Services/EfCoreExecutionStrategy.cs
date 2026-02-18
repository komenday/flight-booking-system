using FBS.Domain.Services;
using FBS.Infrastructure.Persistence;

namespace FBS.Infrastructure.Services;

public class EfCoreExecutionStrategy(ApplicationDbContext dbContext) : IExecutionStrategy
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(operation, (_, op, ct) => op(), null, cancellationToken);
    }
}