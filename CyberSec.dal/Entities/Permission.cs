namespace CyberSec.dal.Entities;

public class Permission
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int MessageId { get; set; }
    public Message Message { get; set; }
    
    public bool CanModify { get; set; }
}