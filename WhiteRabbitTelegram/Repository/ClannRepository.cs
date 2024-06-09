using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public class ClannRepository : IClannRepository
{
    private readonly ApplicationDatabaseContext _context;

    public ClannRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddClann(Clann Clann)
    {
        await _context.Clanns.AddAsync(Clann);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeClann(List<Clann> Clanns)
    {
        await _context.Clanns.AddRangeAsync(Clanns);    
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<Clann>> GetAllClanns()
    {
        return _context.Clanns.AsQueryable();
    }

    public async Task<Clann> GetClannById(Guid id)
    {
        return await _context.Clanns.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Clann> GetClannByOwnerId(Guid id)
    {
        return await _context.Clanns.FirstOrDefaultAsync(i => i.OwnerId == id);
    }

    public async Task<Clann> GetClannByRef(string refLink)
    {
        return await _context.Clanns.FirstOrDefaultAsync(i => i.RefLink == refLink);
    }

    public async Task RemoveClann(Clann Clann)
    {
        _context.Clanns.Remove(Clann);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeClanns(List<Clann> Clanns)
    {
        _context.Clanns.RemoveRange(Clanns);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClann(Clann Clann)
    {
        _context.Clanns.Update(Clann);
        await _context.SaveChangesAsync();
    }
}
