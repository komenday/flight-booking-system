using FBS.Function.Notification.Email;
using FBS.Function.Notifications.Email;
using FBS.Function.Notifications.Interfaces;
using FBS.Function.Notifications.Notification;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.ConfigureFunctionsApplicationInsights();

builder.Services.Configure<MailtrapApiOptions>(
    builder.Configuration.GetSection(MailtrapApiOptions.SectionName));

builder.Services.AddHttpClient<IEmailService, MailtrapApiEmailService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<MailtrapApiOptions>>().Value;

    client.BaseAddress = new Uri("https://sandbox.api.mailtrap.io/");

    var token = options.ApiToken.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
        ? options.ApiToken[7..]
        : options.ApiToken;

    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    client.Timeout = TimeSpan.FromSeconds(30);
})
.AddStandardResilienceHandler();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Build().Run();