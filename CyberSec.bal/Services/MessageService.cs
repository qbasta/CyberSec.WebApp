using CyberSec.bal.Services.Interfaces;
using CyberSec.dal.Context;
using CyberSec.dal.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CyberSec.bal.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly CyberSecDbContext _dbContext;

        public MessagesService(CyberSecDbContext context)
        {
            _dbContext = context;
        }

        public async Task<List<Message>> GetAllMessagesAsync()
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .Include(m => m.Permissions)
                .ThenInclude(p => p.User)
                .OrderByDescending(m => m.CreatedDateTime)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesForUserAsync(string userId)
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .Include(m => m.Permissions)
                .ThenInclude(p => p.User)
                .Where(m => m.UserId == userId)
                .OrderByDescending(m => m.CreatedDateTime)
                .ToListAsync();
        }

        public async Task<List<Message>> GetEditableMessagesForUserAsync(string userId)
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .Include(m => m.Permissions)
                .ThenInclude(p => p.User)
                .Where(m => m.UserId == userId || 
                           m.Permissions.Any(p => p.UserId == userId && p.CanModify))
                .OrderByDescending(m => m.CreatedDateTime)
                .ToListAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            message.Id = Guid.NewGuid().ToString();
            message.CreatedDateTime = DateTime.UtcNow;
            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMessageAsync(string messageId)
        {
            var message = await _dbContext.Messages
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Id == messageId);
            
            if (message != null)
            {
                _dbContext.Permissions.RemoveRange(message.Permissions);
                _dbContext.Messages.Remove(message);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task TogglePermissionAsync(string messageId, string targetUserId)
        {
            var message = await _dbContext.Messages
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null)
            {
                Console.WriteLine($"❌ Wiadomość {messageId} nie została znaleziona.");
                return;
            }

            var permission = message.Permissions.FirstOrDefault(p => p.UserId == targetUserId);

            if (permission != null)
            {
                _dbContext.Permissions.Remove(permission);
                Console.WriteLine($"🚫 Usunięto uprawnienia edycji dla {targetUserId}");
            }
            else
            {
                var newPermission = new Permission
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = targetUserId,
                    MessageId = messageId,
                    CanModify = true
                };
                _dbContext.Permissions.Add(newPermission);
                Console.WriteLine($"✅ Dodano uprawnienia edycji dla {targetUserId}");
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Message?> GetMessageByIdAsync(string messageId)
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .Include(m => m.Permissions)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == messageId);
        }

        public async Task UpdateMessageAsync(string messageId, string newContent)
        {
            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message == null)
            {
                Console.WriteLine($"❌ Wiadomość {messageId} nie została znaleziona.");
                return;
            }

            message.Content = newContent;
            await _dbContext.SaveChangesAsync();

            Console.WriteLine($"✅ Wiadomość {messageId} została zaktualizowana.");
        }

        public async Task<bool> CanUserEditMessageAsync(string userId, string messageId)
        {
            var message = await _dbContext.Messages
                .Include(m => m.Permissions)
                .FirstOrDefaultAsync(m => m.Id == messageId);

            if (message == null) return false;

            return message.UserId == userId || 
                   message.Permissions.Any(p => p.UserId == userId && p.CanModify);
        }

        public async Task<bool> CanUserDeleteMessageAsync(string userId, string messageId)
        {
            var message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.Id == messageId);
            
            if (message == null) return false;

            return message.UserId == userId;
        }
    }
}