using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Command;
using WhiteRabbitTelegram.Extension;
using WhiteRabbitTelegram.Handlers;
using WhiteRabbitTelegram.Keyboard;
using WhiteRabbitTelegram.Repository;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Service;

public class TelegramBotService : ITelegramBotService
{
    private readonly IServiceProvider _srvcProvider;
    private readonly IConfiguration _config;
    private readonly IBaseVisitor _visitor;

    public TelegramBotService(IServiceProvider srvcProvider, IConfiguration config, IBaseVisitor visitor)
    {
        _srvcProvider = srvcProvider;
        _config = config;
        _visitor = visitor;
    }

    public async Task Start(ITelegramBotClient bot, Update upd)
    {
        try
        {
            var chatId = await upd.GetChatId();
            var text = await upd.GetText();
            if (chatId == 0 || string.IsNullOrEmpty(text))
            {
                await bot.SendTextMessageAsync(chatId, "При обработке вашего сообщения что-то пошло не так");
                await Task.CompletedTask;
            }
            else
            {
                await ExecuteCommand(chatId, text, bot, upd);
                await Task.CompletedTask;
            }
        }
        catch (Exception)
        {
            await Task.CompletedTask;
        } 
    }

    private async Task ExecuteCommand(long chatId, string text, ITelegramBotClient bot, Update upd)
    {
        using (var scope = _srvcProvider.CreateScope())
        {
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var user = await userRepository.GetUserByTelegramId(chatId);

            if (user == null)
            {
                user = new Entity.User(chatId, upd.Message.Chat.Username ?? upd.Message.Chat.LastName ?? "unknown user");
                await userRepository.AddUser(user);
            }

            if (text == BotCommands.BackCommand || text == UserCommands.BackCommand)
            {
                var currCommand = user.CurrentCommand;
                user.CurrentCommand = user.LastCommand;
                user.LastCommand = currCommand;
            }

            if (text == UserCommands.StartCommand)
            {
                if (user.IsFirstSign)
                {
                    var referralHandler = new ReferralSystemHandler(user, text, bot, upd);
                    await referralHandler.Accept(_visitor);
                    await bot.SendMessage(upd, user, BotCommands.ConnectWalletCommand, true);
                    user.LastCommand = user.CurrentCommand;
                    user.CurrentCommand = UserCommands.ConnectWalletCommand;
                }
                else
                {
                    await bot.SendMessage(upd, user, BotCommands.CardMainMenuCommand,true, InlineKeyboardButtonMessage.GetButtonsMainMenu());
                    user.CurrentCommand = UserCommands.MainMenuCommand;
                    user.LastCommand = UserCommands.MainMenuCommand;
                }
            }
            else if (user.CurrentCommand == UserCommands.ConnectWalletCommand && user.IsFirstSign)
            {
                var wallet = text;
                if (string.IsNullOrEmpty(wallet))
                {
                    await bot.SendMessage(upd, user, "Вы ввели некорректный адрес кошелька.\n\nВведите корректный адрес кошелька.", false);
                }
                else
                {
                    var handler = new FirstSignHandler(wallet, user, bot, upd);
                    await handler.Accept(_visitor);
                    user.IsFirstSign = false;
                }
            }
            else if (text == UserCommands.MainMenuCommand || text == UserCommands.BackIntoMainMenu)
            {
                await bot.SendMessage(upd, user, BotCommands.CardMainMenuCommand, text == UserCommands.MainMenuCommand ? false:true, InlineKeyboardButtonMessage.GetButtonsMainMenu());
                user.CurrentCommand = UserCommands.MainMenuCommand;
                user.LastCommand = UserCommands.MainMenuCommand;
            }
            else if (text == UserCommands.EarnWBCoinsCommand)
            {
                var earnHandler = new EarnWBCoinsHandler(user, bot, upd);
                await earnHandler.Accept(_visitor);
                user.CurrentCommand = UserCommands.MainMenuCommand;
                user.LastCommand = UserCommands.MainMenuCommand;
            }
            else if(text == UserCommands.EarnWBCoinsByNotificationCommand)
            {
                var earnHandler = new EarnWBCoinsHandler(user, bot, upd);
                await earnHandler.Accept(_visitor);
                await bot.SendMessage(upd, user, BotCommands.CardMainMenuCommand, false, InlineKeyboardButtonMessage.GetButtonsMainMenu());
                user.CurrentCommand = UserCommands.MainMenuCommand;
                user.LastCommand = UserCommands.MainMenuCommand;
            }
            else if (text == UserCommands.PersonalAccountCommand || text == UserCommands.BackIntoPersonalAccountCommand)
            {
                var personalAccountHandler = new PersonalAccountHandler(bot, upd, user, text == UserCommands.PersonalAccountCommand ? true : false);
                await personalAccountHandler.Accept(_visitor);
                user.LastCommand = user.CurrentCommand;
                user.CurrentCommand = text;
            }
            else if (text == UserCommands.TopUsersCommand || text == UserCommands.TopUsersForAllTheTime)
            {
                var topUserHandler = new TopUsersHandler(user, bot, upd, text);
                await topUserHandler.Accept(_visitor);
                user.LastCommand = user.CurrentCommand;
                user.CurrentCommand = text;
            }
            else if (text == UserCommands.ReferralLinkCommand)
            {
                await bot.SendMessage(upd, user, $"Ваша реферальная ссылка - https://t.me/RabbitClubBot?start=whiterabbit{user.OwnReferralId} ❤️‍🔥 \n\n" +
                    $"За каждого приглашенного пользователя 0.01 WC, а за пользователя Telegram Premium 0.02 WC", true, InlineKeyboardButtonMessage.GetButtonReferralLink());
                user.LastCommand = user.CurrentCommand;
                user.CurrentCommand = text;
            }
            else if (text == UserCommands.ChangeWalletAddressCommand)
            {
                await bot.SendMessage(upd, user, "Введите новый адрес кошелька💳", true);
                user.LastCommand = user.CurrentCommand;
                user.CurrentCommand = UserCommands.ConnectNewWalletAddressCommand;
            }
            else if (user.CurrentCommand == UserCommands.ConnectNewWalletAddressCommand)
            {
                var wallet = text;
                if (string.IsNullOrEmpty(wallet))
                {
                    await bot.SendMessage(upd, user, "Вы ввели некорректный адрес кошелька.\n\nВведите корректный адрес кошелька.", true);
                }
                else
                {
                    var handler = new RenewTelegramWalletHandler(user, bot, upd, wallet);
                    await handler.Accept(_visitor);
                }
            }
            user.TelegramId = chatId;
            await userRepository.UpdateUser(user);
        }
    }
}
