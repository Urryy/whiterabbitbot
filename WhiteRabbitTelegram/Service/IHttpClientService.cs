namespace WhiteRabbitTelegram.Service;

public interface IHttpClientService
{
    Task<string> GetJettons(string wallet);
    Task<string> GetNFTs(string wallet);
}
