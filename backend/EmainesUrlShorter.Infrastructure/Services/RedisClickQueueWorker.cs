using System.Text.Json;
using EmainesUrlShorter.Application.Interfaces;
using EmainesUrlShorter.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EmainesUrlShorter.Infrastructure.Services;

public class RedisClickQueueWorker : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RedisClickQueueOptions _options;
    private readonly ILogger<RedisClickQueueWorker> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public RedisClickQueueWorker(
        IConnectionMultiplexer redis,
        IServiceScopeFactory scopeFactory,
        IOptions<RedisClickQueueOptions> options,
        ILogger<RedisClickQueueWorker> logger)
    {
        _redis = redis;
        _scopeFactory = scopeFactory;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var batch = await DequeueBatchAsync(stoppingToken);
            if (batch.Count == 0)
            {
                await Task.Delay(_options.IdleDelayMs, stoppingToken);
                continue;
            }

            await PersistBatchAsync(batch, stoppingToken);
        }
    }

    private async Task<List<LinkAccess>> DequeueBatchAsync(CancellationToken stoppingToken)
    {
        var db = _redis.GetDatabase();
        var batch = new List<LinkAccess>(_options.BatchSize);

        for (var i = 0; i < _options.BatchSize; i++)
        {
            var value = await db.ListLeftPopAsync(_options.ClickQueueKey);
            if (value.IsNullOrEmpty)
            {
                break;
            }

            try
            {
                var access = JsonSerializer.Deserialize<LinkAccess>(value!, JsonOptions);
                if (access != null)
                {
                    batch.Add(access);
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize link access payload.");
            }

            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        return batch;
    }

    private async Task PersistBatchAsync(List<LinkAccess> batch, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IShortLinkRepository>();
            await repository.AddAccessRangeAsync(batch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to persist link access batch.");
            await RequeueBatchAsync(batch, stoppingToken);
        }
    }

    private async Task RequeueBatchAsync(List<LinkAccess> batch, CancellationToken stoppingToken)
    {
        try
        {
            var db = _redis.GetDatabase();
            var payloads = batch
                .Select(access => (RedisValue)JsonSerializer.Serialize(access, JsonOptions))
                .ToArray();

            if (payloads.Length > 0)
            {
                await db.ListRightPushAsync(_options.ClickQueueKey, payloads);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to requeue link access batch.");
        }
    }
}
