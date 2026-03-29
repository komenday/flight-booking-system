using FBS.API.Middlewares;
using FBS.Application;
using FBS.Infrastructure;
using FBS.Infrastructure.Persistence;
using FBS.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

var otlpEndpoint = builder.Configuration["NewRelic:OtlpEndpoint"] ?? "https://otlp.nr-data.net:4318";
var licenseKey = builder.Configuration["NewRelic:LicenseKey"];

if (!string.IsNullOrWhiteSpace(otlpEndpoint) && !string.IsNullOrWhiteSpace(licenseKey))
{
    var serviceName = builder.Configuration["NewRelic:ServiceName"] ?? "FBS-API";

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(serviceName)
            .AddAttributes(new Dictionary<string, object>
            {
                ["environment.name"] = builder.Environment.EnvironmentName
            }))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation(options =>
            {
                options.Filter = ctx =>
                    !ctx.Request.Path.StartsWithSegments("/health");
            })
            .AddHttpClientInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);
                options.Headers = $"api-key={licenseKey}";
            })
        );
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Flight Booking System API",
        Version = "v1",
        Description = "API for flight seat reservations"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNotificationSystem", policy =>
    {
        policy.WithOrigins("https://localhost:5002")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

await dbContext.Database.MigrateAsync();

var seeder = scope.ServiceProvider.GetRequiredService<FlightDataSeeder>();
await seeder.SeedAsync(15);

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Flight Booking System API V1");
    c.RoutePrefix = "swagger";
});

app.UseExceptionHandler();
app.UseCors("AllowNotificationSystem");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"))
        .ExcludeFromDescription();
}

await app.RunAsync();