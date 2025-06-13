namespace CyberSec.dal.Entities;

public class User
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}