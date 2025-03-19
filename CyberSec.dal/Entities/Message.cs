namespace CyberSec.dal.Entities;

public class Message
{
    public int Id { get; set; }
    public string Content { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }

    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}