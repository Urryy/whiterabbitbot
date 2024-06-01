using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;
using TonSdk.Client;

namespace WhiteRabbitTelegram.Service;

public class HttpClientService : IHttpClientService
{
    private readonly IConfiguration _configuration;

    public HttpClientService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<string> GetJettons(string wallet)
    {
        var url = new Uri($"https://tonapi.io/v2/accounts/{wallet}/jettons");
        var baseUrl = new Uri("https://tonapi.io/v2/");
        var handler = new HttpClientHandler();

        using (var client = new HttpClient(handler) { BaseAddress = baseUrl })
        {
            return await client.GetAsync(url)
                .Result.Content.ReadAsStringAsync();
        }
    }

    public async Task<string> GetNFTs(string wallet)
    {
        int page = 0;
        var url = new Uri($"https://tonapi.nftscan.com/api/ton/account/own/all/{wallet}?show_attribute=false");
        var baseUrl = new Uri("https://tonapi.nftscan.com/");
        var handler = new HttpClientHandler();

        using (var client = new HttpClient(handler) { BaseAddress = baseUrl })
        {
            client.DefaultRequestHeaders.Add("X-API-KEY", _configuration["TonNftApiKey"]);
            return await client.GetAsync(url)
                .Result.Content.ReadAsStringAsync();
        }
    }
}
