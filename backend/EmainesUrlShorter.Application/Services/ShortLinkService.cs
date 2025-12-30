using EmainesUrlShorter.Application.Dto;
using EmainesUrlShorter.Application.Interfaces;
using EmainesUrlShorter.Domain.Entities;

namespace EmainesUrlShorter.Application.Services;

public class ShortLinkService : IShortLinkService
{
    private readonly IShortLinkRepository _repository;

    private const string Alphabet = "0ivyEh4TtlnI9GLYsKCpwcVquWjXeSArHbm1OdPNMZ8RaxUfF25oQ36kD7BJgz";
    private static readonly int Base = Alphabet.Length;

    private const string BaseUrl = "https://emaines.sh/"; 

    public ShortLinkService(IShortLinkRepository repository)
    {
        _repository = repository;
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
            OriginalUrl = l.OriginalUrl
        });
    }

    public async Task<string?> GetOriginalUrlAsync(string code)
    {
        var link = await _repository.GetByCodeAsync(code);
        return link?.OriginalUrl;
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
}
