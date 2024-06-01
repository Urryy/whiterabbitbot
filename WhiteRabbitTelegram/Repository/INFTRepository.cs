namespace WhiteRabbitTelegram.Repository;

public interface INFTRepository
{
    Task<IQueryable<Entity.NFT>> GetAllNfts();
    Task AddRangeNft(List<Entity.NFT> nfts);
    Task AddNft(Entity.NFT nft);
    Task<List<Entity.NFT>> GetNFTsByUserId(Guid userId);
    Task RemoveRangeNfts(List<Entity.NFT> nfts);
    Task RemoveNft(Entity.NFT nft);
    Task UpdateNft(Entity.NFT nft);
}
