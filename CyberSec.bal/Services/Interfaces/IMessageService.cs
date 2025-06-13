using CyberSec.dal.Entities;

namespace CyberSec.bal.Services.Interfaces
{
    public interface IMessagesService
    {
        Task<List<Message>> GetAllMessagesAsync();
        Task<List<Message>> GetMessagesForUserAsync(string userId);
        Task<List<Message>> GetEditableMessagesForUserAsync(string userId);
        Task AddMessageAsync(Message message);
        Task DeleteMessageAsync(string messageId);
        Task TogglePermissionAsync(string messageId, string targetUserId);
        Task<Message?> GetMessageByIdAsync(string messageId);
        Task UpdateMessageAsync(string messageId, string newContent);
        Task<bool> CanUserEditMessageAsync(string userId, string messageId);
        Task<bool> CanUserDeleteMessageAsync(string userId, string messageId);
    }
}