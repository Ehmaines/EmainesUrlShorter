namespace EmainesUrlShorter.Application.Dto;

public class ShortLinkResponse
{
    public string Code { get; set; } = string.Empty;
    public string ShortUrl { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public int TotalClicks { get; set; } = 0;
}
