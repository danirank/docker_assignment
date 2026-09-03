using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;

namespace ScanTrackNode.Services;

public class HeartbeatService(
    NodeRegistry registry,
    IConfiguration configuration,
    ILogger<HeartbeatService> _logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var city = configuration["CITY_NAME"];
        var nodeUrl = configuration["NODE_URL"];
        var registryUrl = configuration["REGISTRY_URL"];

        if (string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(nodeUrl) ||
            string.IsNullOrWhiteSpace(registryUrl))
        {
            _logger.LogError("Miljövariabler för heartbeat saknas.");
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Väntar en timme innan nästa heartbeat
                await Task.Delay(TimeSpan.FromHours(1), ct);

                _logger.LogInformation("Skickar heartbeat till {Registry}", registryUrl);

                await registry.RegisterSelfAsync();
            }
            catch (TaskCanceledException)
            {
                // Ignorera TaskCanceledException som kastas när tjänsten stoppas
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kunde inte skicka heartbeat till {Registry}", registryUrl);
            }
        }
    }
}