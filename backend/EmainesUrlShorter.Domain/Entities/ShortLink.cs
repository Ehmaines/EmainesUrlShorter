
namespace EmainesUrlShorter.Domain.Entities;

public class ShortLink
{
    public int Id { get; set; }
    public string OriginalUrl { get; set; } = string.Empty; 
    public string Code { get; set; } = string.Empty;
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<LinkAccess> Accesses { get; set; } = new List<LinkAccess>();
}
