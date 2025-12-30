namespace EmainesUrlShorter.API.Models;

public record CreateShortLinkRequest(string OriginalUrl, Guid OwnerId);
