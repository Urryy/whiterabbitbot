namespace WhiteRabbitTelegram.Extension;

public static class CacheExtension
{
    public static string GetCacheKeyNFT(string wallet) => $"{wallet}_NFT";
    public static string GetCacheKeyJetton(string wallet) => $"{wallet}_JETTON";
}
