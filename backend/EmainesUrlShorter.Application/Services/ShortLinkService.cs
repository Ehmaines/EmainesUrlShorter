using System.Collections.Concurrent;
using System.Net;
using EmainesUrlShorter.Application.Dto;
using EmainesUrlShorter.Application.Interfaces;
using EmainesUrlShorter.Domain.Entities;

namespace EmainesUrlShorter.Application.Services;

public class ShortLinkService : IShortLinkService
{
    private readonly IShortLinkRepository _repository;
    private readonly IClickTracker _clickTracker;
    private static readonly HttpClient GeoIpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(2)
    };
    private static readonly ConcurrentDictionary<string, string> CountryCache = new(StringComparer.Ordinal);

    private const string Alphabet = "0ivyEh4TtlnI9GLYsKCpwcVquWjXeSArHbm1OdPNMZ8RaxUfF25oQ36kD7BJgz";
    private static readonly int Base = Alphabet.Length;

    private const string BaseUrl = "http://localhost:5266/"; 

    public ShortLinkService(IShortLinkRepository repository, IClickTracker clickTracker)
    {
        _repository = repository;
        _clickTracker = clickTracker;
    }

    public async Task<ShortLinkResponse> ShortenAsync(ShortenUrlRequest request)
    {
        if (!Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid URL format");
        }

        var shortLink = new ShortLink
        {
            OriginalUrl = request.OriginalUrl,
            Code = Guid.NewGuid().ToString().Substring(0, 8),
            OwnerId = Guid.Parse(request.OwnerId),
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(shortLink);

        string code = EncodeBase62(shortLink.Id);
        shortLink.Code = code;

        await _repository.UpdateAsync(shortLink);
        
        return new ShortLinkResponse
        {
            Code = code,
            ShortUrl = $"{BaseUrl}{code}",
            OriginalUrl = shortLink.OriginalUrl
        };
    }

    public async Task<IEnumerable<ShortLinkResponse>> GetByOwnerAsync(string ownerId)
    {
        if (!Guid.TryParse(ownerId, out var ownerGuid))
        {
            return Enumerable.Empty<ShortLinkResponse>();
        }

        var links = await _repository.GetByOwnerAsync(ownerGuid);

        return links.Select(l => new ShortLinkResponse
        {
            Code = l.Code,
            ShortUrl = $"{BaseUrl}{l.Code}",
            OriginalUrl = l.OriginalUrl,
            TotalClicks = l.TotalClicks
        });
    }

    public async Task<string?> GetOriginalUrlAsync(string code)
    {
        var link = await _repository.GetByCodeAsync(code);
        return link?.OriginalUrl;
    }

    public async Task<string?> GetOriginalUrlAndTrackAsync(string code, string? userAgent, string? ipAddress)
    {
        var link = await _repository.GetByCodeAsync(code);
        if (link == null)
        {
            return null;
        }

        var country = await GetCountryByIpAsync(ipAddress);
        var access = new LinkAccess
        {
            LinkId = link.Id,
            Browser = DetectBrowser(userAgent),
            Device = DetectDevice(userAgent),
            Country = country
        };

        await _clickTracker.TrackAsync(access);

        return link.OriginalUrl;
    }

    private string EncodeBase62(int id)
    {
        if (id == 0) return Alphabet[0].ToString();

        var sb = new System.Text.StringBuilder();
        while (id > 0)
        {
            sb.Insert(0, Alphabet[id % Base]);
            id /= Base;
        }

        return sb.ToString();
    }

    private static string DetectBrowser(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "Unknown";
        }

        if (userAgent.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
        {
            return "Edge";
        }

        if (userAgent.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
        {
            return "Chrome";
        }

        if (userAgent.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
        {
            return "Firefox";
        }

        if (userAgent.Contains("Safari/", StringComparison.OrdinalIgnoreCase))
        {
            return "Safari";
        }

        return "Other";
    }

    private static string DetectDevice(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
        {
            return "Unknown";
        }

        if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
            userAgent.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
        {
            return "Tablet";
        }

        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
        {
            return "Mobile";
        }

        return "Desktop";
    }

    private static async Task<string> GetCountryByIpAsync(string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return "Unknown";
        }

        if (CountryCache.TryGetValue(ipAddress, out var cached))
        {
            return cached;
        }

        if (!IPAddress.TryParse(ipAddress, out var ip) || !IsPublicIp(ip))
        {
            CountryCache[ipAddress] = "Unknown";
            return "Unknown";
        }

        try
        {
            using var response = await GeoIpClient.GetAsync($"https://ipapi.co/{ipAddress}/country_name/");
            if (!response.IsSuccessStatusCode)
            {
                CountryCache[ipAddress] = "Unknown";
                return "Unknown";
            }

            var country = (await response.Content.ReadAsStringAsync()).Trim();
            if (string.IsNullOrWhiteSpace(country) || string.Equals(country, "Undefined", StringComparison.OrdinalIgnoreCase))
            {
                CountryCache[ipAddress] = "Unknown";
                return "Unknown";
            }

            CountryCache[ipAddress] = country;
            return country;
        }
        catch (HttpRequestException)
        {
            CountryCache[ipAddress] = "Unknown";
            return "Unknown";
        }
        catch (TaskCanceledException)
        {
            CountryCache[ipAddress] = "Unknown";
            return "Unknown";
        }
    }

    private static bool IsPublicIp(IPAddress ip)
    {
        if (IPAddress.IsLoopback(ip))
        {
            return false;
        }

        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
        {
            var bytes = ip.GetAddressBytes();
            return bytes[0] switch
            {
                10 => false,
                127 => false,
                169 when bytes[1] == 254 => false,
                172 when bytes[1] is >= 16 and <= 31 => false,
                192 when bytes[1] == 168 => false,
                _ => true
            };
        }

        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            if (ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast)
            {
                return false;
            }

            var bytes = ip.GetAddressBytes();
            if ((bytes[0] & 0xFE) == 0xFC)
            {
                return false;
            }

            return true;
        }

        return false;
    }
}
