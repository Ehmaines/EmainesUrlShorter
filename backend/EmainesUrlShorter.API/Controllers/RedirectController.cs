using EmainesUrlShorter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmainesUrlShorter.API.Controllers;

[ApiController]
[Route("/{code}")]
public class RedirectController : ControllerBase
{
    private readonly IShortLinkService _service;

    public RedirectController(IShortLinkService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> RedirectShortCode(string code)
    {
        var userAgent = Request.Headers.UserAgent.ToString();
        var ipAddress = GetClientIpAddress();
        var originalUrl = await _service.GetOriginalUrlAndTrackAsync(code, userAgent, ipAddress);
        if (originalUrl == null)
        {
            return NotFound();
        }

        return Redirect(originalUrl);
    }

    private string? GetClientIpAddress()
    {
        if (Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            var firstIp = forwardedFor.ToString().Split(',', 2)[0].Trim();
            if (!string.IsNullOrWhiteSpace(firstIp))
            {
                return firstIp;
            }
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
