namespace CyberSec.dal.Entities;

public class Message
{
    public string Id { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public DateTime CreatedDateTime { get; set; }
    
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}