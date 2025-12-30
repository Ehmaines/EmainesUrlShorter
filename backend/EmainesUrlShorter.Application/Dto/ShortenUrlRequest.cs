namespace EmainesUrlShorter.Application.Dto;

public class ShortenUrlRequest
{
    public string OriginalUrl { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
}
