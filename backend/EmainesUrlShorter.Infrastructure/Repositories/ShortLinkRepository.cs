using EmainesUrlShorter.Application.Interfaces;
using EmainesUrlShorter.Domain.Entities;
using EmainesUrlShorter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmainesUrlShorter.Infrastructure.Repositories;

public class ShortLinkRepository : IShortLinkRepository
{
    private readonly AppDbContext _context;

    public ShortLinkRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ShortLink?> GetByCodeAsync(string code)
    {
        return await _context.ShortLinks
            .FirstOrDefaultAsync(s => s.Code == code);
    }

    public async Task<IEnumerable<ShortLink>> GetByOwnerAsync(Guid ownerId)
    {
        return await _context.ShortLinks
            .Where(s => s.OwnerId == ownerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ShortLink shortLink)
    {
        await _context.ShortLinks.AddAsync(shortLink);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ShortLink shortLink)
    {
        _context.ShortLinks.Update(shortLink);
        await _context.SaveChangesAsync();
    }

    public async Task AddAccessAsync(LinkAccess access)
    {
        await _context.LinkAccesses.AddAsync(access);
        await _context.SaveChangesAsync();
    }

    public async Task AddAccessRangeAsync(IEnumerable<LinkAccess> accesses)
    {
        var accessList = accesses as IList<LinkAccess> ?? accesses.ToList();
        if (accessList.Count == 0)
        {
            return;
        }

        await _context.LinkAccesses.AddRangeAsync(accessList);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.ShortLinks
            .AnyAsync(s => s.Code == code);
    }
}
