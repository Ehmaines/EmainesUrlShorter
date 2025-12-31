using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<EmainesUrlShorter.Infrastructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EmainesUrlShorter.Application.Interfaces.IShortLinkRepository, EmainesUrlShorter.Infrastructure.Repositories.ShortLinkRepository>();
builder.Services.AddScoped<EmainesUrlShorter.Application.Interfaces.IShortLinkService, EmainesUrlShorter.Application.Services.ShortLinkService>();
builder.Services.Configure<EmainesUrlShorter.Infrastructure.Services.RedisClickQueueOptions>(
    builder.Configuration.GetSection("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var options = sp.GetRequiredService<IOptions<EmainesUrlShorter.Infrastructure.Services.RedisClickQueueOptions>>().Value;
    return ConnectionMultiplexer.Connect(options.ConnectionString);
});
builder.Services.AddSingleton<EmainesUrlShorter.Application.Interfaces.IClickTracker, EmainesUrlShorter.Infrastructure.Services.RedisClickTracker>();
builder.Services.AddHostedService<EmainesUrlShorter.Infrastructure.Services.RedisClickQueueWorker>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();

