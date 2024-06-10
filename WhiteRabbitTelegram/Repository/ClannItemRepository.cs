using Microsoft.EntityFrameworkCore;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Repository;

public class ClannItemRepository : IClannItemRepository
{
    private readonly ApplicationDatabaseContext _context;
    public ClannItemRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }
    public async Task AddClannItem(ClannItem ClannItem)
    {
        await _context.ClannItems.AddAsync(ClannItem);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeClannItem(List<ClannItem> ClannItems)
    {
        await _context.ClannItems.AddRangeAsync(ClannItems);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<ClannItem>> GetAllClannItems()
    {
        return _context.ClannItems.AsQueryable();
    }

    public async Task<List<ClannItem>> GetClannItemByClannId(Guid id)
    {
        return await _context.ClannItems.Where(i => i.ClannId == id).ToListAsync();
    }

    public async Task<ClannItem> GetClannItemByUserId(Guid id)
    {
        return await _context.ClannItems.FirstOrDefaultAsync(i => i.UserId == id);
    }

    public async Task RemoveClannItem(ClannItem ClannItem)
    {
        _context.ClannItems.Remove(ClannItem);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveRangeClannItems(List<ClannItem> ClannItems)
    {
        _context.ClannItems.RemoveRange(ClannItems);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateClannItem(ClannItem ClannItem)
    {
        _context.ClannItems.Update(ClannItem);
        await _context.SaveChangesAsync();
    }
}
