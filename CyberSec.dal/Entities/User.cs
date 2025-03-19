namespace CyberSec.dal.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}