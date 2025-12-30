using EmainesUrlShorter.Application.Dto;

namespace EmainesUrlShorter.Application.Interfaces;

public interface IShortLinkService
{
    Task<ShortLinkResponse> ShortenAsync(ShortenUrlRequest request);
    Task<IEnumerable<ShortLinkResponse>> GetByOwnerAsync(string ownerId);
    Task<string?> GetOriginalUrlAsync(string code);
}
