using Telegram.Bot.Types.ReplyMarkups;
using WhiteRabbitTelegram.Command;
using WhiteRabbitTelegram.Entity;

namespace WhiteRabbitTelegram.Keyboard;

public static class InlineKeyboardButtonMessage
{
    public static InlineKeyboardMarkup GetButtonsMainMenu(Role role, int page = 1)
    {
        if(role == Role.Admin)
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Добыть", UserCommands.EarnWBCoinsCommand),
                    InlineKeyboardButton.WithCallbackData("Топ", UserCommands.TopUsersCommand)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Личный кабинет", UserCommands.PersonalAccountCommand)
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Все пользователи", UserCommands.AllUsersCommand(1))
                }
            });
        }
        else
        {
            return new InlineKeyboardMarkup(new[]
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
        }
    }

    public static InlineKeyboardMarkup GetButtonAlllUsersNavigation(bool hasNextPage, bool hasPreviousPage, int page)
    {
        if(hasNextPage && hasPreviousPage) 
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                { 
                    InlineKeyboardButton.WithCallbackData("Назад", UserCommands.AllUsersCommand(page - 1)),
                    InlineKeyboardButton.WithCallbackData("Вперед", UserCommands.AllUsersCommand(page + 1))
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
                }
            });
        }
        else if (hasNextPage)
        {
            return new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Вперед", UserCommands.AllUsersCommand(page + 1))
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
                }
            });
        }
        else
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад", UserCommands.AllUsersCommand(page - 1))
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Назад в главное меню", UserCommands.BackIntoMainMenu)
                }
            });
        }
    }

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

    public static InlineKeyboardMarkup GetButtonPersonalAccount(Clann? isOwner) => new InlineKeyboardMarkup(new[]
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
        isOwner != null 
        ? new[]
        {
            InlineKeyboardButton.WithCallbackData("Информация о клане", UserCommands.InformationAboutClannCommand)
        }
        : new[]
        {
            InlineKeyboardButton.WithCallbackData("Создать клан", UserCommands.CreateClannCommand)
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Изменить адрес кошелька", UserCommands.ChangeWalletAddressCommand)
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

    public static InlineKeyboardMarkup GetButtonChangeTelegramWallet() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в Личный кабинет", UserCommands.BackIntoPersonalAccountCommand)
        }
    });

    public static InlineKeyboardMarkup GetButtonBackIntoPersonalAccount() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в Личный кабинет", UserCommands.BackIntoPersonalAccountCommand)
        }
    });

    public static InlineKeyboardMarkup GetButtonCreateClann() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Создать", UserCommands.ConfirmCreateClannCommand)
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Назад в Личный кабинет", UserCommands.BackIntoPersonalAccountCommand)
        }
    });

    public static InlineKeyboardMarkup GetButtonEarnCoins() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Добыть", UserCommands.EarnWBCoinsByNotificationCommand),
        }
    });

    public static InlineKeyboardMarkup GetButtonIsMemberChannel() => new InlineKeyboardMarkup(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithUrl("White Rabbit", "https://t.me/WhiteRabbitTON")
        },
        new[]
        {
            InlineKeyboardButton.WithCallbackData("Проверить подписку", UserCommands.CheckSubscribeCommand)
        }
    });
}
