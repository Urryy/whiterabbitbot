using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDatabaseContext _context;
    public UserRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddRangeUser(List<Entity.User> users)
    {
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public async Task AddUser(Entity.User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<Entity.User>> GetAllUsers()
    {
        return _context.Users.AsQueryable();
    }

    public async Task<Entity.User> GetUserByTelegramId(long id)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.TelegramId == id);
    }

    public async Task<Entity.User> GetUserById(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Entity.User> GetUserByTelegramWallet(string wallet)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.TelegramWallet == wallet);
    }

    public async Task RemoveRangeUsers(List<Entity.User> users)
    {
        _context.Users.RemoveRange(users);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUser(Entity.User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(Entity.User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<Entity.User> GetUserByRef(string referral)
    {
        return await _context.Users.FirstOrDefaultAsync(i => i.OwnReferralId == referral);
    }
}
