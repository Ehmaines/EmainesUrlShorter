
namespace EmainesUrlShorter.Domain.Entities;

public class LinkAccess
{
    public int Id { get; set; }
    public int LinkId { get; set; }
    public ShortLink? ShortLink { get; set; }
    public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    public string Country { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Device { get; set; } = string.Empty;
}
