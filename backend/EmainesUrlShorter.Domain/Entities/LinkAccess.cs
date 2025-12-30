
namespace EmainesUrlShorter.Domain.Entities;

public class LinkAccess
{
    public int Id { get; set; }
    public int ShortLinkId { get; set; }
    public ShortLink? ShortLink { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
