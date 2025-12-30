using EmainesUrlShorter.Domain.Entities;

namespace EmainesUrlShorter.Application.Interfaces;

public interface IShortLinkRepository
{
    Task<ShortLink?> GetByCodeAsync(string code);
    Task<IEnumerable<ShortLink>> GetByOwnerAsync(Guid ownerId);
    Task AddAsync(ShortLink shortLink);
    Task UpdateAsync(ShortLink shortLink);
    Task<bool> CodeExistsAsync(string code);
}
