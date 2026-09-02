using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;

namespace ScanTrackNode.Services;

public class HeartbeatService(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<HeartbeatService> logger)
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
            logger.LogError("Miljövariabler för heartbeat saknas.");
            return;
        }

        while (!ct.IsCancellationRequested)
        {
            try
            {
                // Väntar en timme innan nästa heartbeat
                await Task.Delay(TimeSpan.FromHours(1), ct);

                var client = httpClientFactory.CreateClient();

                var response = await client.PostAsJsonAsync(
                    $"{registryUrl.TrimEnd('/')}/nodes",
                    new
                    {
                        city,
                        url = nodeUrl
                    },
                    ct);

                response.EnsureSuccessStatusCode();

                logger.LogInformation(
                    "Heartbeat skickad för {City} till registryt.",
                    city);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                // Normalt när containern stoppas
            }
            catch (Exception ex)
            {
                // Ett tillfälligt fel ska inte stoppa tjänsten permanent
                logger.LogError(ex, "Kunde inte skicka heartbeat.");
            }
        }
    }
}