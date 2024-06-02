using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using TonSdk.Client;
using WhiteRabbitTelegram.Command;
using WhiteRabbitTelegram.Entity;
using WhiteRabbitTelegram.Extension;
using WhiteRabbitTelegram.Handlers;
using WhiteRabbitTelegram.Keyboard;
using WhiteRabbitTelegram.Records;
using WhiteRabbitTelegram.Repository;
using WhiteRabbitTelegram.Service;

namespace WhiteRabbitTelegram.Visitor;

public class BaseVisitor : IBaseVisitor
{
    private readonly IHttpClientService _httpClientService;
    private readonly IServiceProvider _srvcProvider;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;

    public BaseVisitor(IHttpClientService httpClientService, IServiceProvider srvcProvider, IMemoryCache cache, IConfiguration configuration)
    {
        _httpClientService = httpClientService;
        _srvcProvider = srvcProvider;
        _cache = cache;
        _configuration = configuration;
    }

    private async Task SetBalance(string wallet, Entity.User user)
    {
        var jettonsString = await _httpClientService.GetJettons(wallet);
        var jettons = JsonConvert.DeserializeObject<JettonsRecord>(jettonsString);
        user.SetBalance(jettons);
        _cache.Set(CacheExtension.GetCacheKeyJetton(user.TelegramWallet), user.TokensWhiteCoin,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(double.Parse(_configuration["ExpiresCache"]!))));
        await Task.CompletedTask;
    }

    private async Task SetNFTs(string wallet, Entity.User user)
    {
        var nftsString = await _httpClientService.GetNFTs(wallet);
        var nfts = JsonConvert.DeserializeObject<NftsRecord>(nftsString);
        var nftsEntities = new List<NFT>();
        if (nfts != null && nfts.code == 200 && nfts.data.Length != 0)
        {
            using (var scope = _srvcProvider.CreateScope())
            {
                var nftsRecords = nfts.data.FirstOrDefault(item => item.contract_address == "EQB_utiDZBuWgQWGib8SV2XYVbWy8KVsRaTMT7la0Q5V8jP4");
                if (nftsRecords != null)
                {
                    var _nftRepository = scope.ServiceProvider.GetRequiredService<INFTRepository>();
                    
                    var nftsByUserId = (await _nftRepository.GetNFTsByWallet(user.TelegramWallet)).ToList();
                    if(nftsByUserId.Count > 0)
                    {
                        user.CountNFT = 0;
                        await _nftRepository.RemoveRangeNfts(nftsByUserId);
                    }

                    if(user.CountNFT == null)
                    {
                        user.CountNFT = 0;
                    }
                    
                    foreach (var nft in nftsRecords.assets)
                    {
                        user.CountNFT += 1;
                        nftsEntities.Add(new NFT(nft.token_address, nft.contract_name, nft.contract_address, nft.token_id, nft.block_number, nft.minter, nft.owner,
                            nft.mint_timestamp, nft.mint_transaction_hash, nft.mint_price, nft.token_uri, nft.metadata_json, nft.name, nft.content_type, nft.content_uri,
                            nft.image_uri, nft.description, user.TelegramWallet!));
                    }
                    await _nftRepository.AddRangeNft(nftsEntities);
                    
                }
            }
        }
        _cache.Set(CacheExtension.GetCacheKeyNFT(user.TelegramWallet), nftsEntities,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(double.Parse(_configuration["ExpiresCache"]!))));
    }

    public async Task Visit(EarnWBCoinsHandler handler)
    {
        var chatId = await handler.upd.GetCallbackQueryId();
        using (var scope = _srvcProvider.CreateScope())
        {
            var nfts = await scope.ServiceProvider.GetRequiredService<INFTRepository>().GetNFTsByWallet(handler.user.TelegramWallet);
            if (DateTime.Compare(handler.user.DateCreated.Date, handler.user.DateUpdated.Date) == 0 && handler.user.DateCreated.Hour == handler.user.DateUpdated.Hour
                && handler.user.DateCreated.Minute == handler.user.DateUpdated.Minute && handler.user.DateCreated.Second == handler.user.DateUpdated.Second)
            {
                var repositoryWB = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                var tokens = (decimal)(0.01 * (nfts.Count == 0 ? 1 : nfts.Count));
                var token = new TokenWС(tokens, handler.user.TelegramWallet!);
                await repositoryWB.AddTokenWC(token);
                await handler.bot.AnswerCallbackQueryAsync(chatId, $"Поздравляем!\n\nВы заработали свои первые {token.Tokens.ToString()} WB coins!", showAlert: true);
                handler.user.DateUpdated = DateTime.UtcNow;
            }
            else
            {

                var diffDate = DateTime.UtcNow.Subtract(handler.user.DateUpdated);
                if (diffDate.TotalHours >= 6)
                {
                    var repositoryWB = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                    var token = new TokenWС((decimal)(0.01 * (nfts.Count == 0 ? 1 : nfts.Count)), handler.user.TelegramWallet!);
                    await repositoryWB.AddTokenWC(token);
                    await handler.bot.AnswerCallbackQueryAsync(chatId, $"Вы получили {token.Tokens.ToString()} WB coins!", showAlert: true);
                    handler.user.DateUpdated = DateTime.UtcNow;
                }
                else
                {
                    var nextEarnDate = handler.user.DateUpdated.AddHours(6);
                    await handler.bot.AnswerCallbackQueryAsync(chatId, $"Можете добыть в {nextEarnDate.ToString("t")}", showAlert: true);
                }
            }
        }
        await Task.CompletedTask;
    }

    public async Task Visit(TopUsersHandler handler)
    {
        var count = 1;
        var currUserInTop = true;
        var placeInTop = 0;

        var strBuild = new StringBuilder();
        if (handler.command == UserCommands.TopUsersCommand)
        {
            using (var scope = _srvcProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var tokenWBRepository = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                var tokens = (await tokenWBRepository.GetAllTokenWCs())
                    .Where(i => i.DateEarn >= DateTime.UtcNow.AddDays(-14))
                    .GroupBy(k => k.Wallet)
                    .Select(g => new
                    {
                        Wallet = g.Key,
                        TotalCoins = g.Sum(p => p.Tokens)
                    })
                    .OrderByDescending(s => s.TotalCoins)
                    .ToList();
                
                strBuild.AppendLine("Топ пользователей за две недели:");
                foreach(var token in tokens)
                {
                    var user = await userRepository.GetUserByTelegramWallet(token.Wallet);
                    if(user != null)
                    {
                        if (user.Id == handler.user.Id && count > 10)
                        {
                            currUserInTop = false;
                            placeInTop = count;
                        }
                        strBuild.AppendLine($"{count}. {user.Name} - {token.TotalCoins}");
                        count++;
                    }
                }
                if(currUserInTop == false)
                {
                    strBuild.AppendLine($"\nТы не в топе, но находишься на {placeInTop} месте.");
                }
            }
            await handler.bot.SendTextMessageAsync(await handler.upd.GetChatId(), strBuild.ToString(), replyMarkup: InlineKeyboardButtonMessage.GetButtonTopUsers());
        }
        else
        {
            using (var scope = _srvcProvider.CreateScope())
            {
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var tokenWBRepository = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                var tokens = (await tokenWBRepository.GetAllTokenWCs())
                    .GroupBy(k => k.Wallet)
                    .Select(g => new
                    {
                        Wallet = g.Key,
                        TotalCoins = g.Sum(p => p.Tokens)
                    })
                    .OrderByDescending(s => s.TotalCoins)
                    .ToList();

                strBuild.AppendLine("Топ пользователей:");
                foreach(var token in tokens)
                {
                    var user = await userRepository.GetUserByTelegramWallet(token.Wallet);
                    if (user != null)
                    {
                        if (user.Id == handler.user.Id && count > 10)
                        {
                            currUserInTop = false;
                            placeInTop = count;
                        }
                        strBuild.AppendLine($"{count}. {user.Name} - {token.TotalCoins}");
                        count++;
                    }
                }
                if (currUserInTop == false)
                {
                    strBuild.AppendLine($"\nТы не в топе, но находишься на {placeInTop} месте.");
                }
            }
            await handler.bot.SendTextMessageAsync(await handler.upd.GetChatId(), strBuild.ToString(), replyMarkup: InlineKeyboardButtonMessage.GetButtonAllTImeTopUsers());
        }
    }

    public async Task Visit(PersonalAccountHandler handler)
    {
        using (var scope = _srvcProvider.CreateScope())
        {
            var chatId = await handler.upd.GetChatId();

            if(!_cache.TryGetValue(CacheExtension.GetCacheKeyNFT(handler.user.TelegramWallet), out List<NFT> nfts))
            {
                await handler.bot.SendTextMessageAsync(chatId, BotCommands.AgainCheckNftCollectionCommand);
                await SetNFTs(handler.user.TelegramWallet!, handler.user);
            }

            if(!_cache.TryGetValue(CacheExtension.GetCacheKeyJetton(handler.user.TelegramWallet), out decimal balance))
            {
                await handler.bot.SendTextMessageAsync(chatId, BotCommands.CheckWhiteCoinsCommand);
                await SetBalance(handler.user.TelegramWallet!, handler.user);
            }

            var nftRepository = scope.ServiceProvider.GetRequiredService<INFTRepository>();
            var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();

            var tokensByWallet = await tokenRepository.GetTokenWCByWallet(handler.user.TelegramWallet);
            var nftsByWallet = (await nftRepository.GetNFTsByWallet(handler.user.TelegramWallet)).Count;
            var message = $"Количество NFT: {nftsByWallet}\n\n" +
                $"Количество купленных WhiteCoins: {Math.Round(handler.user.TokensWhiteCoin, 2)}\n\n" +
                $"За все время вы добыли {tokensByWallet.Select(i => i.Tokens).Sum()} токенов WC\n\n" +
                $"Количество пользователей присоиденившихся по вашей реферальной ссылке: {handler.user.Referrals}";

            await handler.bot.SendTextMessageAsync(chatId, message, replyMarkup: InlineKeyboardButtonMessage.GetButtonPersonalAccount());
        }
    }

    public async Task Visit(ReferralSystemHandler handler)
    {
        if (handler.textWithRefLink.Contains("whiterabbit"))
        {
            using (var scope = _srvcProvider.CreateScope())
            {
                var userRep = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                var tokenWBRep = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();

                int startIndex = handler.textWithRefLink.IndexOf("whiterabbit");
                if (startIndex >= 0)
                {
                    string result = handler.textWithRefLink.Substring(startIndex + "whiterabbit".Length);
                    var userByRefLink = await userRep.GetUserByRef(result);
                    if(userByRefLink != null && handler.user.OwnReferralId != result)
                    {
                        if (userByRefLink.TelegramWallet != null)
                        {
                            userByRefLink.Referrals += 1;
                            var isPremium = await handler.upd.IsTelegramPremium();
                            var wb = new TokenWС(isPremium ? (decimal)0.01 : (decimal)0.02, userByRefLink.TelegramWallet);
                            await tokenWBRep.AddTokenWC(wb);
                            await userRep.UpdateUser(userByRefLink);
                        }
                    }
                }
            }
        }
        await Task.CompletedTask;
    }

    public async Task Visit(RenewTelegramWalletHandler handler)
    {
        var chatId = await handler.upd.GetChatId();

        handler.user.TelegramWallet = handler.wallet;

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckNFTCollectionCommand;
        await handler.bot.SendTextMessageAsync(chatId, BotCommands.CheckNFTCollectionCommand);
        await SetNFTs(handler.wallet, handler.user);

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckWhiteCoinsCommand;
        await handler.bot.SendTextMessageAsync(chatId, BotCommands.CheckWhiteCoinsCommand);
        await SetBalance(handler.wallet, handler.user);

        await handler.bot.SendTextMessageAsync(chatId, "Вы успешно изменили свой адрес кошелька", 
            replyMarkup: InlineKeyboardButtonMessage.GetButtonChangeTelegramWallet());

        await Task.CompletedTask;
    }

    public async Task Visit(FirstSignHandler handler)
    {
        var chatId = await handler.upd.GetChatId();
        handler.user.TelegramWallet = handler.wallet;

        //Проверка кошелька и проверка на наличие NFT коллекции
        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckWhiteCoinsCommand;

        await handler.bot.SendTextMessageAsync(chatId, BotCommands.CheckWhiteCoinsCommand);
        await SetBalance(handler.wallet, handler.user);

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckNFTCollectionCommand;

        await handler.bot.SendTextMessageAsync(chatId, BotCommands.CheckNFTCollectionCommand);
        await SetNFTs(handler.wallet, handler.user);

        //Вывод сообщения о том, что есть ли бонусы при фарме токенов.
        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.MainMenuCommand;

        await handler.bot.SendTextMessageAsync(chatId, BotCommands.MainMenuCommand);
        await Task.Delay(1000);
        await handler.bot.SendTextMessageAsync(chatId, BotCommands.CardMainMenuCommand, replyMarkup: InlineKeyboardButtonMessage.GetButtonsMainMenu());
    }
}
