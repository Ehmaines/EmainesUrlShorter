using System.Text.Json;
using EmainesUrlShorter.Application.Interfaces;
using EmainesUrlShorter.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace EmainesUrlShorter.Infrastructure.Services;

public class RedisClickTracker : IClickTracker
{
    private readonly IConnectionMultiplexer _redis;
    private readonly RedisClickQueueOptions _options;
    private readonly ILogger<RedisClickTracker> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public RedisClickTracker(
        IConnectionMultiplexer redis,
        IOptions<RedisClickQueueOptions> options,
        ILogger<RedisClickTracker> logger)
    {
        _redis = redis;
        _options = options.Value;
        _logger = logger;
    }

    public async Task TrackAsync(LinkAccess access, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var payload = JsonSerializer.Serialize(access, JsonOptions);
            await db.ListRightPushAsync(_options.ClickQueueKey, payload);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to enqueue link access to Redis.");
        }
    }
}
