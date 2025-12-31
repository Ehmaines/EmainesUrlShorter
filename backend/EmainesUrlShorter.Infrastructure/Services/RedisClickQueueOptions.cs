namespace EmainesUrlShorter.Infrastructure.Services;

public class RedisClickQueueOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public string ClickQueueKey { get; set; } = "link-access-queue";
    public int BatchSize { get; set; } = 200;
    public int IdleDelayMs { get; set; } = 500;
}
