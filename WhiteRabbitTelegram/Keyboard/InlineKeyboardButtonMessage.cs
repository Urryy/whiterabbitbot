using Telegram.Bot.Types.ReplyMarkups;
using WhiteRabbitTelegram.Command;

namespace WhiteRabbitTelegram.Keyboard;

public static class InlineKeyboardButtonMessage
{
    public static InlineKeyboardMarkup GetButtonsMainMenu() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Добыть", UserCommands.EarnWBCoinsCommand),
            InlineKeyboardButton.WithCallbackData("Топ", UserCommands.TopUsersCommand)
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Личный кабинет", UserCommands.PersonalAccountCommand)
        }
    });

    public static InlineKeyboardMarkup GetButtonTopUsers() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("За все время", UserCommands.TopUsersForAllTheTime),
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад", UserCommands.BackIntoMainMenu)
        }
    });

    public static InlineKeyboardMarkup GetButtonAllTImeTopUsers() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
        }
    });

    public static InlineKeyboardMarkup GetButtonPersonalAccount() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithWebApp("Купить NFT", new Telegram.Bot.Types.WebAppInfo() 
            {
                Url = "https://getgems.io/whiteclub" 
            })
        },
        new[]
        {
            InlineKeyboardButton.WithWebApp("Купить WhiteCoin",new Telegram.Bot.Types.WebAppInfo() 
            {
                Url = "https://app.ston.fi/swap?chartVisible=false&ft=TON&tt=EQB-h24QEzN23Z1EOlExXTzDYza4VdKVRfJ_-nfBMa_xBybP" 
            })
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Пригласить пользователя", UserCommands.ReferralLinkCommand)
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
        }
    });

    public static InlineKeyboardMarkup GetButtonReferralLink() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
        }
    });
}
