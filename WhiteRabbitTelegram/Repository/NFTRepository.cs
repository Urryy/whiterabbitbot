using WhiteRabbitTelegram.Entity;
using WhiteRabbitTelegram.Records;

namespace WhiteRabbitTelegram.Repository;

public class NFTRepository : INFTRepository
{
    private readonly ApplicationDatabaseContext _context;

    public NFTRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task AddRangeNft(List<NFT> nfts)
    {
        await _context.NFTs.AddRangeAsync(nfts);
        await _context.SaveChangesAsync();
    }

    public async Task AddNft(NFT nft)
    {
        await _context.NFTs.AddAsync(nft);
        await _context.SaveChangesAsync();
    }

    public async Task<IQueryable<NFT>> GetAllNfts()
    {
        return _context.NFTs.AsQueryable();
    }

    public async Task<List<NFT>> GetNFTsByUserId(Guid userId)
    {
        return _context.NFTs.Where(i => i.UserId == userId).ToList();
    }

    public async Task RemoveRangeNfts(List<NFT> nfts)
    {
        _context.NFTs.RemoveRange(nfts);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveNft(NFT nft)
    {
        _context.NFTs.Remove(nft);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateNft(NFT nft)
    {
        _context.Update(nft);
        await _context.SaveChangesAsync();
    }
}
