using Telegram.Bot;
using Telegram.Bot.Types;

namespace WhiteRabbitTelegram.Service;

public interface ITelegramBotService
{
    Task Start(ITelegramBotClient bot, Update upd);
    Task StartTest(ITelegramBotClient bot, Update upd);

}
