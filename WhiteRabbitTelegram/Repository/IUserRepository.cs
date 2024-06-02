using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public interface IUserRepository
{
    Task<IQueryable<Entity.User>> GetAllUsers();
    Task AddRangeUser(List<Entity.User> users);
    Task AddUser(Entity.User user);
    Task RemoveRangeUsers(List<Entity.User> users);
    Task RemoveUser(Entity.User user);
    Task UpdateUser(Entity.User user);
    Task<Entity.User> GetUserByTelegramId(long id);
    Task<Entity.User> GetUserById(Guid id);
    Task<Entity.User> GetUserByRef(string referral);
    Task<Entity.User> GetUserByTelegramWallet(string wallet);
}
