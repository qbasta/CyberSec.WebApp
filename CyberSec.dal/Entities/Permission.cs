namespace CyberSec.dal.Entities;

public class Permission
{
    public string Id { get; set; } = string.Empty;
    
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    
    public string MessageId { get; set; } = string.Empty;
    public Message Message { get; set; } = null!;
    
    public bool CanModify { get; set; }
}