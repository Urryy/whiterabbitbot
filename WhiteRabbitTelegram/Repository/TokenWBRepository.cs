using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public class TokenWBRepository : ITokenWBRepository
{
    private readonly ApplicationDatabaseContext _context;
    public TokenWBRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }
    public async Task AddRangeTokenWB(List<TokenWB> TokenWBs)
    {
        await _context.TokenWBs.AddRangeAsync(TokenWBs);
        await _context.SaveChangesAsync();
    }

    public async Task AddTokenWB(TokenWB TokenWB)
    {
        await _context.TokenWBs.AddAsync(TokenWB);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<TokenWB>> GetAllTokenWBs()
    {
        return _context.TokenWBs.AsQueryable();
    }

    public async Task<List<TokenWB>> GetTokenWBByUserId(Guid id)
    {
        return await _context.TokenWBs.Where(item => item.UserId == id).ToListAsync();
    }

    public async Task RemoveRangeTokenWBs(List<TokenWB> TokenWBs)
    {
        _context.TokenWBs.RemoveRange(TokenWBs);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveTokenWB(TokenWB TokenWB)
    {
        _context.TokenWBs.Remove(TokenWB);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTokenWB(TokenWB TokenWB)
    {
        _context.TokenWBs.Update(TokenWB);
        await _context.SaveChangesAsync();
    }
}
