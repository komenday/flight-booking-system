using FBS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Application.Common.Behaviors;

public class TransactionBehavior<TRequest, TResponse>
    (IUnitOfWork unitOfWork, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);
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
}