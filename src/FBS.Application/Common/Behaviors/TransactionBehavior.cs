using FBS.Domain.Repositories;
using FBS.Domain.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>
    (IUnitOfWork unitOfWork,
     IExecutionStrategy executionStrategy,
     ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IExecutionStrategy _executionStrategy = executionStrategy;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        if (IsQuery(requestName))
        {
            return await next(cancellationToken);
        }

        _logger.LogInformation("Beginning transaction for {RequestName}", requestName);

        return await _executionStrategy.ExecuteAsync(async () =>
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next(cancellationToken);

                if (!IsSuccessResult(response))
                {
                    _logger.LogWarning("Rolling back transaction for {RequestName} - operation returned failure result", requestName);

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return response;
                }

                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogWarning(ex, "Concurrency conflict in {RequestName} - rolling back transaction", requestName);

                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                    return CreateConcurrencyFailureResult<TResponse>(
                        "A concurrency conflict occurred. The resource was modified by another user. Please retry.");
                }

                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                _logger.LogInformation("Committed transaction for {RequestName}", requestName);

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rolling back transaction for {RequestName} due to exception", requestName);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }, cancellationToken);
    }

    private static bool IsQuery(string requestName)
    {
        return requestName.EndsWith("Query", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSuccessResult(TResponse response)
    {
        if (response is null)
            return false;

        var type = response.GetType();
        var isSuccessProperty = type.GetProperty("IsSuccess");

        if (isSuccessProperty is not null)
        {
            return (bool)(isSuccessProperty.GetValue(response) ?? false);
        }

        return true;
    }

    private static TResponse CreateConcurrencyFailureResult<T>(string errorMessage)
    {
        var resultType = typeof(T);

        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition().Name.StartsWith("Result"))
        {
            var conflictMethod = typeof(Result.Result)
                .GetMethods()
                .FirstOrDefault(m =>
                    m.Name == "Conflict" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);

            if (conflictMethod is not null)
            {
                var genericMethod = conflictMethod.MakeGenericMethod(resultType.GetGenericArguments());
                var result = genericMethod.Invoke(null, [errorMessage, null]);
                return (TResponse)result!;
            }
        }
        else if (resultType == typeof(Result.Result))
        {
            var result = Result.Result.Conflict(errorMessage);
            return (TResponse)(object)result;
        }

        throw new InvalidOperationException($"Cannot create failure result for type {resultType.Name}");
    }
}