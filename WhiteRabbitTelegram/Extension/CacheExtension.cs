namespace WhiteRabbitTelegram.Extension;

public static class CacheExtension
{
    public static string GetCacheKeyNFT(Guid Id) => $"{Id}_NFT";
    public static string GetCacheKeyJetton(Guid Id) => $"{Id}_JETTON";
}
