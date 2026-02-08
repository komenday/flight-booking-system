using FBS.Application.Commands.ExpireReservation;
using FBS.Domain.Common.Specifications;
using FBS.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FBS.Infrastructure.BackgroundJobs;

public class ExpireReservationsJob(
    IReservationRepository reservationRepository,
    ISender sender,
    ILogger<ExpireReservationsJob> logger)
{
    private readonly IReservationRepository _reservationRepository = reservationRepository;
    private readonly ISender _sender = sender;
    private readonly ILogger<ExpireReservationsJob> _logger = logger;

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("ExpireReservationsJob started at {ExecutionTime}", DateTime.UtcNow);

        try
        {
            var spec = new ExpiredReservationsSpecification();
            var expiredReservations = await _reservationRepository.GetAsync(spec, cancellationToken);

            if (!expiredReservations.Any())
            {
                _logger.LogInformation("No expired reservations found");
                return;
            }

            _logger.LogInformation("Found {Count} expired reservations to process", expiredReservations.Count());

            var processedCount = 0;
            var failedCount = 0;

            foreach (var reservation in expiredReservations)
            {
                try
                {
                    var command = new ExpireReservationCommand(reservation.Id.Value);
                    var result = await _sender.Send(command, cancellationToken);

                    if (result.IsSuccess)
                    {
                        processedCount++;
                        _logger.LogDebug("Successfully expired reservation {ReservationId}", reservation.Id.Value);
                    }
                    else
                    {
                        failedCount++;
                        _logger.LogWarning("Failed to expire reservation {ReservationId}: {Error}", reservation.Id.Value, result.Error);
                    }
                }
                catch (Exception ex)
                {
                    failedCount++;
                    _logger.LogError(ex, "Error processing expired reservation {ReservationId}", reservation.Id.Value);
                }
            }

            _logger.LogInformation("ExpireReservationsJob completed. Processed: {ProcessedCount}, Failed: {FailedCount}", processedCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error in ExpireReservationsJob");
            throw;
        }
    }
}
