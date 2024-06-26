﻿using Microsoft.Extensions.Caching.Memory;
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
using static System.Formats.Asn1.AsnWriter;

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
        var nftsEntities = new List<NFT>();
        var nftsString = await _httpClientService.GetNFTsFromTONAPI(wallet);
        using (var scope = _srvcProvider.CreateScope())
        {
            var _nftRepository = scope.ServiceProvider.GetRequiredService<INFTRepository>();

            var nftsByUserId = (await _nftRepository.GetNFTsByWallet(user.TelegramWallet)).ToList();
            if (nftsByUserId.Count > 0)
            {
                user.CountNFT = 0;
                await _nftRepository.RemoveRangeNfts(nftsByUserId);
            }

            if (!string.IsNullOrEmpty(nftsString))
            {
                var nfts = JsonConvert.DeserializeObject<NftsItemsRecord>(nftsString);
                if (nfts != null && nfts.nft_items != null && nfts.nft_items.Length != 0)
                {

                    var nftsRecords = nfts.nft_items.Where(item => item.collection != null && item.collection.address != null &&
                    item.collection.address == "0:7fbad883641b9681058689bf125765d855b5b2f0a56c45a4cc4fb95ad10e55f2");
                    if (nftsRecords.Count() > 0)
                    {
                        if (user.CountNFT == null)
                        {
                            user.CountNFT = 0;
                        }

                        foreach (var nft in nftsRecords)
                        {
                            try
                            {
                                user.CountNFT += 1;
                                nftsEntities.Add(new NFT(nft.address, nft.collection?.name, nft.address, nft.address, nft.index, nft.owner?.name, nft.owner?.address,
                                    nft.index, nft.metadata?.name, decimal.One, nft.metadata?.image, nft.collection?.name, nft.metadata?.name, nft.collection?.name, nft.metadata?.marketplace,
                                    nft.metadata?.image, nft.collection?.name, user.TelegramWallet!));
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            
                        }
                        await _nftRepository.AddRangeNft(nftsEntities);
                    }

                }
            }
        }
        
        
        _cache.Set(CacheExtension.GetCacheKeyNFT(user.TelegramWallet), nftsEntities,
                        new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(double.Parse(_configuration["ExpiresCache"]!))));
    }

    private async Task JoinClann(Entity.User user, string text, string indexName = "clann")
    {
        using (var scope = _srvcProvider.CreateScope())
        {
            int startIndex = text.IndexOf(indexName);
            if (startIndex >= 0)
            {
                string result = text.Substring(startIndex + indexName.Length);

                var clannRepository = scope.ServiceProvider.GetRequiredService<IClannRepository>();
                var clannItemRepository = scope.ServiceProvider.GetRequiredService<IClannItemRepository>();

                var clannByRefLink = await clannRepository.GetClannByRef(result);
                var userIsInClann = await clannItemRepository.GetClannItemByUserId(user.Id);
                if (clannByRefLink != null && userIsInClann == null)
                {
                    var item = new ClannItem(user.Id, clannByRefLink.Id, decimal.Zero);
                    await clannItemRepository.AddClannItem(item);
                }
            }
        }
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
                var clannItemRepository = scope.ServiceProvider.GetRequiredService<IClannItemRepository>();
                var repositoryWB = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                var nftMultiplier = nfts.Count == 0 ? 1 : (nfts.Count * 3);
                var tokens = (decimal)(0.01 * nftMultiplier);

                var clannItem = await clannItemRepository.GetClannItemByUserId(handler.user.Id);
                if (clannItem != null)
                {
                    var clannRepository = scope.ServiceProvider.GetRequiredService<IClannRepository>();
                    var clann = await clannRepository.GetClannById(clannItem.ClannId);
                    if (clann != null)
                    {
                        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                        var owner = await userRepository.GetUserById(clann.OwnerId);
                        var tokenForOwner = (handler.user.CountNFT != null && handler.user.CountNFT > 0) 
                            ? (tokens * (decimal)0.1) : (tokens * (decimal)0.05);
                        await repositoryWB.AddTokenWC(new TokenWС(tokenForOwner, owner.TelegramWallet!));
                        clannItem.EarnedCoins = decimal.Add(tokenForOwner, clannItem.EarnedCoins);
                        await clannItemRepository.UpdateClannItem(clannItem);
                    }
                }

                var token = new TokenWС(tokens, handler.user.TelegramWallet!);
                await repositoryWB.AddTokenWC(token);
                await handler.bot.AnswerCallbackQueryAsync(chatId, $"Поздравляем!\n\nВы заработали свои первые {token.Tokens.ToString()} WC coins!", showAlert: true);
                handler.user.DateUpdated = DateTime.Now;
                handler.user.IsSendedNotification = false;
            }
            else
            {

                var diffDate = DateTime.Now.Subtract(handler.user.DateUpdated);
                if (diffDate.TotalHours >= 6)
                {
                    var clannItemRepository = scope.ServiceProvider.GetRequiredService<IClannItemRepository>();
                    var repositoryWB = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
                    var nftMultiplier = nfts.Count == 0 ? 1 : (nfts.Count * 3);
                    var tokens = (decimal)(0.01 * nftMultiplier);

                    var clannItem = await clannItemRepository.GetClannItemByUserId(handler.user.Id);
                    if (clannItem != null)
                    {
                        var clannRepository = scope.ServiceProvider.GetRequiredService<IClannRepository>();
                        var clann = await clannRepository.GetClannById(clannItem.ClannId);
                        if (clann != null)
                        {
                            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                            var owner = await userRepository.GetUserById(clann.OwnerId);
                            var tokenForOwner = (handler.user.CountNFT != null && handler.user.CountNFT > 0)
                                ? (tokens * (decimal)0.1) : (tokens * (decimal)0.05);
                            await repositoryWB.AddTokenWC(new TokenWС(tokenForOwner, owner.TelegramWallet!));
                            clannItem.EarnedCoins = decimal.Add(tokenForOwner, clannItem.EarnedCoins);
                            await clannItemRepository.UpdateClannItem(clannItem);
                        }
                    }

                    var token = new TokenWС(tokens, handler.user.TelegramWallet!);
                    await repositoryWB.AddTokenWC(token);
                    await handler.bot.AnswerCallbackQueryAsync(chatId, $"Вы получили {token.Tokens.ToString()} WC coins!", showAlert: true);
                    handler.user.DateUpdated = DateTime.Now;
                    handler.user.IsSendedNotification = false;
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
                    .Where(i => i.DateEarn >= DateTime.Now.AddDays(-14))
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

                        if (count < 16)
                        {
                            strBuild.AppendLine($"{count}. {user.Name} - {token.TotalCoins}");
                        }
                        count++;
                    }
                }
                if(currUserInTop == false)
                {
                    strBuild.AppendLine($"\nТы не в топе, но находишься на {placeInTop} месте.");
                }
            }
            await handler.bot.SendMessage(handler.upd, handler.user, strBuild.ToString(), true, InlineKeyboardButtonMessage.GetButtonTopUsers());
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

                        if (count < 16)
                        {
                            strBuild.AppendLine($"{count}. {user.Name} - {token.TotalCoins}");
                        }

                        count++;
                    }
                }
                if (currUserInTop == false)
                {
                    strBuild.AppendLine($"\nТы не в топе, но находишься на {placeInTop} месте.");
                }
            }
            await handler.bot.SendMessage(handler.upd, handler.user, strBuild.ToString(), true, InlineKeyboardButtonMessage.GetButtonAllTImeTopUsers());
        }
    }

    public async Task Visit(PersonalAccountHandler handler)
    {
        using (var scope = _srvcProvider.CreateScope())
        {
            var chatId = await handler.upd.GetChatId();

            if(!_cache.TryGetValue(CacheExtension.GetCacheKeyNFT(handler.user.TelegramWallet), out List<NFT> nfts))
            {
                await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.AgainCheckNftCollectionCommand, true);
                await SetNFTs(handler.user.TelegramWallet!, handler.user);
            }

            if(!_cache.TryGetValue(CacheExtension.GetCacheKeyJetton(handler.user.TelegramWallet), out decimal balance))
            {
                await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CheckWhiteCoinsCommand, true);
                await SetBalance(handler.user.TelegramWallet!, handler.user);
            }

            var nftRepository = scope.ServiceProvider.GetRequiredService<INFTRepository>();
            var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();

            var tokensByWallet = await tokenRepository.GetTokenWCByWallet(handler.user.TelegramWallet);
            var nftsByWallet = (await nftRepository.GetNFTsByWallet(handler.user.TelegramWallet)).Count;
            var message = $"В вашем профиле:\n\n" +
                $"- {nftsByWallet} NFT\n" +
                $"- Добыто {tokensByWallet.Select(i => i.Tokens).Sum()} WC токенов\n" +
                $"- Куплено {Math.Round(handler.user.TokensWhiteCoin, 2)} WhiteCoins\n\n" +
                $"Количество пользователей присоединившихся по вашей реферальной ссылке: {handler.user.Referrals}";

            var clannRepository = scope.ServiceProvider.GetRequiredService<IClannRepository>();
            var isOwnerClann = await clannRepository.GetClannByOwnerId(handler.user.Id);
            await handler.bot.SendMessage(handler.upd, handler.user, message, handler.isReplaceMessage, InlineKeyboardButtonMessage.GetButtonPersonalAccount(isOwnerClann));
        }
    }

    public async Task Visit(AllUsersHandler handler)
    {
        var chatId = await handler.upd.GetChatId();
        var strBuilder = new StringBuilder();
        var count = 1;
        using (var scope = _srvcProvider.CreateScope())
        {
            int startIndex = handler.command.IndexOf("AllUsersCommand_");
            if(startIndex >= 0)
            {
                string page = handler.command.Substring(startIndex + "AllUsersCommand_".Length);
                if(int.TryParse(page, out var pageInt))
                {
                    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    var tokenRepo = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();

                    var allUsers = (await userRepo.GetAllUsers()).OrderByDescending(i => i.CountNFT);
                    var activeUsersCount = allUsers.Where(i => i.TelegramWallet != null).Count();
                    var paginatedUsers = await PaginationExtension<Entity.User>.CreateAsync(allUsers, pageInt, 15);
                    strBuilder.AppendLine($"Количество пользователей на данный момент: {allUsers.Count()}\n");
                    strBuilder.AppendLine($"Количество активных пользователей: {activeUsersCount}\n");
                    strBuilder.AppendLine($"Пользователи:");

                    foreach (var user in paginatedUsers.Items)
                    {
                        var tokens = await tokenRepo.GetTokenWCByWallet(user.TelegramWallet);
                        strBuilder.AppendLine($"{count}. {user.Name} | {user.CountNFT ?? 0} NFT | Добыто {tokens.Select(i => i.Tokens).Sum()} WC | Куплено {user.TokensWhiteCoin} WC");
                        count++;
                    }

                    await handler.bot.SendMessage(handler.upd, handler.user, strBuilder.ToString(), true,
                        InlineKeyboardButtonMessage.GetButtonAlllUsersNavigation(paginatedUsers.HasNextPage, paginatedUsers.HasPreviousPage, paginatedUsers.Page));
                }
            }   
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
                            var wb = new TokenWС(isPremium ? (decimal)1 : (decimal)0.5, userByRefLink.TelegramWallet);
                            await tokenWBRep.AddTokenWC(wb);
                            await userRep.UpdateUser(userByRefLink);
                        }
                    }
                    await JoinClann(handler.user, handler.textWithRefLink, "whiterabbit");
                }
            }
        }
        await Task.CompletedTask;
    }

    public async Task Visit(RenewTelegramWalletHandler handler)
    {
        handler.user.TelegramWallet = handler.wallet;

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckNFTCollectionCommand;
        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CheckNFTCollectionCommand, true);
        await SetNFTs(handler.wallet, handler.user);

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckWhiteCoinsCommand;
        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CheckWhiteCoinsCommand, true);
        await SetBalance(handler.wallet, handler.user);

        await handler.bot.SendMessage(handler.upd, handler.user, "Вы успешно изменили свой адрес кошелька", true, InlineKeyboardButtonMessage.GetButtonChangeTelegramWallet());

        await Task.CompletedTask;
    }

    public async Task Visit(FirstSignHandler handler)
    {
        handler.user.TelegramWallet = handler.wallet;

        //Проверка кошелька и проверка на наличие NFT коллекции
        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckWhiteCoinsCommand;

        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CheckWhiteCoinsCommand, true);
        await SetBalance(handler.wallet, handler.user);

        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.CheckNFTCollectionCommand;

        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CheckNFTCollectionCommand, true);
        await SetNFTs(handler.wallet, handler.user);

        //Вывод сообщения о том, что есть ли бонусы при фарме токенов.
        handler.user.LastCommand = handler.user.CurrentCommand;
        handler.user.CurrentCommand = UserCommands.MainMenuCommand;

        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.MainMenuCommand, true);
        await Task.Delay(1000);
        await handler.bot.SendMessage(handler.upd, handler.user, BotCommands.CardMainMenuCommand, false, InlineKeyboardButtonMessage.GetButtonsMainMenu(handler.user.Role));
    }
    
    public async Task Visit(CreateClannHandler handler)
    {
        var chatId = handler.upd.GetChatId();
        using (var scope = _srvcProvider.CreateScope())
        {
            var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenWCRepository>();
            var tokensByUserId = await tokenRepository.GetTokenWCByWallet(handler.user.TelegramWallet);
            var sumOfTokens = tokensByUserId.Select(i => i.Tokens).Sum();
            if(sumOfTokens < 30)
            {
                var callId = await handler.upd.GetCallbackQueryId();
                await handler.bot.AnswerCallbackQueryAsync(callId, $"Для создания клана требуется добыть 30 WC токенов", showAlert: true);
                return;
            }

            var clann = new Clann(handler.user.Id, handler.user.OwnReferralId);
            var clannRepo = scope.ServiceProvider.GetRequiredService<IClannRepository>();
            await clannRepo.AddClann(clann);

            var tokensToRemove = new List<TokenWС>();
            sumOfTokens = 0;
            foreach (var token in tokensByUserId)
            {
                sumOfTokens += token.Tokens;
                if(sumOfTokens >= 30)
                {
                    var ostatokSumi = 30 - (sumOfTokens - token.Tokens);
                    var newSum = token.Tokens - ostatokSumi;
                    await tokenRepository.AddTokenWC(new TokenWС(newSum, handler.user.TelegramWallet));
                    tokensToRemove.Add(token);
                    break;
                }
                else
                {
                    tokensToRemove.Add(token);
                }
            }
            await tokenRepository.RemoveRangeTokenWCs(tokensToRemove);
        }
        await handler.bot.SendMessage(handler.upd, handler.user, "Вы успешно создали свой клан", true, InlineKeyboardButtonMessage.GetButtonBackIntoPersonalAccount());
    }

    public async Task Visit(JoinClannHandler handler)
    {
        await JoinClann(handler.user, handler.text);
    }

    public async Task Visit(InformationAboutClannHandler handler)
    {
        var chatId = await handler.upd.GetChatId();
        using (var scope = _srvcProvider.CreateScope())
        {
            var clanRepository = scope.ServiceProvider.GetRequiredService<IClannRepository>();
            var clann = await clanRepository.GetClannByOwnerId(handler.user.Id);
            if (clann != null)
            {
                var strBuilder = new StringBuilder();
                var clannItemRepository = scope.ServiceProvider.GetRequiredService<IClannItemRepository>();
                var clannItems = await clannItemRepository.GetClannItemByClannId(clann.Id);

                strBuilder.AppendLine("Информацие о вашем клане:🐇\n");
                strBuilder.AppendLine($"Ссылка для вступления в клан - https://t.me/RabbitClubBot?start=clann{clann.RefLink}\n");
                strBuilder.Append("Пользователи: ");
                if (clannItems.Count != 0)
                {
                    
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    strBuilder.Append($"{clannItems.Count}\n\n");
                    strBuilder.AppendLine("🚀 Топ 10 пользователей по добыче монет 🚀");
                    int count = 0;
                    foreach (var item in clannItems.OrderByDescending(i => i.EarnedCoins))
                    {
                        if (count >= 10)
                            break;
                        var user = await userRepository.GetUserById(item.UserId);
                        strBuilder.AppendLine($"- {user.Name} | {item.EarnedCoins}");
                        count++;
                    }
                }
                else
                {
                    strBuilder.AppendLine("В данный момент в вашем клане нету пользователей");
                }
                await handler.bot.SendMessage(handler.upd, handler.user, strBuilder.ToString(), true, InlineKeyboardButtonMessage.GetButtonBackIntoPersonalAccount());
            }
        }
    }
}
