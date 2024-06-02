using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public class TokenWCRepository : ITokenWCRepository
{
    private readonly ApplicationDatabaseContext _context;
    public TokenWCRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }
    public async Task AddRangeTokenWC(List<TokenWС> TokenWCs)
    {
        await _context.TokenWCs.AddRangeAsync(TokenWCs);
        await _context.SaveChangesAsync();
    }

    public async Task AddTokenWC(TokenWС TokenWC)
    {
        await _context.TokenWCs.AddAsync(TokenWC);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<TokenWС>> GetAllTokenWCs()
    {
        return _context.TokenWCs.AsQueryable();
    }

    public async Task<List<TokenWС>> GetTokenWCByWallet(string wallet)
    {
        return await _context.TokenWCs.Where(item => item.Wallet == wallet).ToListAsync();
    }

    public async Task RemoveRangeTokenWCs(List<TokenWС> TokenWCs)
    {
        _context.TokenWCs.RemoveRange(TokenWCs);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveTokenWC(TokenWС TokenWC)
    {
        _context.TokenWCs.Remove(TokenWC);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTokenWC(TokenWС TokenWC)
    {
        _context.TokenWCs.Update(TokenWC);
        await _context.SaveChangesAsync();
    }
}
