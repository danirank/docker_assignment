using System.Net.Http.Json;
using Microsoft.Extensions.Hosting;

namespace ScanTrackNode.Services;

public class HeartbeatService : BackgroundService
{
    private readonly NodeRegistry _registry;
    private readonly ILogger<HeartbeatService> _logger;
    private DateTime _lastHeartbeat = DateTime.MinValue;
    private static readonly TimeSpan MinInterval = TimeSpan.FromMinutes(10);

    public HeartbeatService(NodeRegistry registry, ILogger<HeartbeatService> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            await SendHeartbeatAsync();
        }
    }
    // Anropas både av den schemalagda loopen och av /forceheartbeat
    public async Task<bool> SendHeartbeatAsync()
    {
        _lastHeartbeat = DateTime.UtcNow;
        _logger.LogInformation("Skickar heartbeat...");
        await _registry.RegisterSelfAsync();
        return true;
    }

    // Anropas av /forceheartbeat — nekar om det var för nyligen
    public async Task<bool> TryForceHeartbeatAsync()
    {
        if (DateTime.UtcNow - _lastHeartbeat < MinInterval)
        {
            _logger.LogWarning("Forcerat heartbeat nekat — för kort tid sedan senaste.");
            return false;
        }

        await SendHeartbeatAsync();
        return true;
    }

}



    
