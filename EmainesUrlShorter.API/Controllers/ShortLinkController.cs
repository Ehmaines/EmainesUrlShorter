using EmainesUrlShorter.API.Models;
using EmainesUrlShorter.Domain.Entities;
using EmainesUrlShorter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmainesUrlShorter.API.Controllers;

[ApiController]
[Route("api/short-links")]
public class ShortLinkController : ControllerBase
{
    private readonly AppDbContext _context;

    public ShortLinkController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateShortLinkRequest request)
    {
        // Simple code generation logic (for demo purposes, real world needs collision handling)
        var code = GenerateCode();

        var shortLink = new ShortLink
        {
            OriginalUrl = request.OriginalUrl,
            OwnerId = request.OwnerId,
            Code = code
        };

        _context.ShortLinks.Add(shortLink);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(RedirectToOriginal), new { code = code }, new { code, shortLink.OriginalUrl });
    }

    [HttpGet("/{code}")]
    public async Task<IActionResult> RedirectToOriginal(string code)
    {
        var shortLink = await _context.ShortLinks
            .FirstOrDefaultAsync(s => s.Code == code);

        if (shortLink == null)
        {
            return NotFound();
        }

        // Log access
        var access = new LinkAccess
        {
            ShortLinkId = shortLink.Id,
            ShortLink = shortLink
        };
        _context.LinkAccesses.Add(access);
        await _context.SaveChangesAsync();

        return Redirect(shortLink.OriginalUrl);
    }

    private string GenerateCode()
    {
        // Simple random string generator
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
