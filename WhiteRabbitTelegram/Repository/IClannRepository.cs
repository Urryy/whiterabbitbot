namespace WhiteRabbitTelegram.Repository;

public interface IClannRepository
{
    Task<IQueryable<Entity.Clann>> GetAllClanns();
    Task AddRangeClann(List<Entity.Clann> Clanns);
    Task AddClann(Entity.Clann Clann);
    Task RemoveRangeClanns(List<Entity.Clann> Clanns);
    Task RemoveClann(Entity.Clann Clann);
    Task UpdateClann(Entity.Clann Clann);
    Task<Entity.Clann> GetClannByOwnerId(Guid id);
    Task<Entity.Clann> GetClannByRef(string refLink);
    Task<Entity.Clann> GetClannById(Guid id);
    
}
