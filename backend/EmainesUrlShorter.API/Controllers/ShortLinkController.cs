using EmainesUrlShorter.Application.Dto;
using EmainesUrlShorter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmainesUrlShorter.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShortLinkController : ControllerBase
{
    private readonly IShortLinkService _service;

    public ShortLinkController(IShortLinkService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Shorten([FromBody] ShortenUrlRequest request)
    {
        if (string.IsNullOrEmpty(request.OriginalUrl))
        {
            return BadRequest("URL is required.");
        }

        try
        {
            var result = await _service.ShortenAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("by-owner/{ownerId}")]
    public async Task<IActionResult> GetByOwner(string ownerId)
    {
        var result = await _service.GetByOwnerAsync(ownerId);
        return Ok(result);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> RedirectLink(string code)
    {
        var originalUrl = await _service.GetOriginalUrlAsync(code);
        if (originalUrl == null)
        {
            return NotFound();
        }

        return Redirect(originalUrl);
    }
}
