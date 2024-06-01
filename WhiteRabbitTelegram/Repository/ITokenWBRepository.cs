namespace WhiteRabbitTelegram.Repository;

public interface ITokenWBRepository
{
    Task<IQueryable<Entity.TokenWB>> GetAllTokenWBs();
    Task AddRangeTokenWB(List<Entity.TokenWB> TokenWBs);
    Task AddTokenWB(Entity.TokenWB TokenWB);
    Task RemoveRangeTokenWBs(List<Entity.TokenWB> TokenWBs);
    Task RemoveTokenWB(Entity.TokenWB TokenWB);
    Task UpdateTokenWB(Entity.TokenWB TokenWB);
    Task<List<Entity.TokenWB>> GetTokenWBByUserId(Guid id);
}
