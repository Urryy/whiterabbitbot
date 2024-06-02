using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public interface ITokenWCRepository
{
    Task<IQueryable<Entity.TokenWС>> GetAllTokenWCs();
    Task AddRangeTokenWC(List<Entity.TokenWС> TokenWCs);
    Task AddTokenWC(Entity.TokenWС TokenWC);
    Task RemoveRangeTokenWCs(List<Entity.TokenWС> TokenWCs);
    Task RemoveTokenWC(Entity.TokenWС TokenWC);
    Task UpdateTokenWC(Entity.TokenWС TokenWC);
    Task<List<TokenWС>> GetTokenWCByWallet(string wallet);
}
