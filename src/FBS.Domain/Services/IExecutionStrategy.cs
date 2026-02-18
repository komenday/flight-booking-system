namespace FBS.Domain.Services;

public interface IExecutionStrategy
{
    Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);
}