using FBS.Infrastructure.BackgroundJobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FBS.Function.ExpiredReservations.Functions;

public class ExpireReservationsFunction(
    ExpireReservationsJob expireReservationsJob,
    ILogger<ExpireReservationsFunction> logger)
{
    private readonly ExpireReservationsJob _expireReservationsJob = expireReservationsJob;
    private readonly ILogger<ExpireReservationsFunction> _logger = logger;

    [Function(nameof(ExpireReservations))]
    public async Task ExpireReservations(
        [TimerTrigger("0 */2 * * * *", RunOnStartup = false)] TimerInfo timerInfo,
        CancellationToken cancellationToken)
    {
        if (timerInfo.IsPastDue)
        {
            _logger.LogWarning("ExpireReservations timer is running late. Last execution: {LastRun}", timerInfo.ScheduleStatus?.Last);
        }

        _logger.LogInformation("ExpireReservations timer triggered. Next scheduled run: {NextRun}", timerInfo.ScheduleStatus?.Next);

        await _expireReservationsJob.ExecuteAsync(cancellationToken);
    }
}
