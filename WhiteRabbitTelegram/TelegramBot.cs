using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Service;

namespace WhiteRabbitTelegram;

public class TelegramBot
{
    private TelegramBotClient _bot;
    private readonly ITelegramBotService _srvcTelegramBot;
    private readonly IConfiguration _config;
    public TelegramBot(ITelegramBotService srvcTelegramBot, IConfiguration config)
    {
        _srvcTelegramBot = srvcTelegramBot;
        _config = config;
    }

    public async Task UpdateHandler(ITelegramBotClient bot, Update upd, CancellationToken token)
    {
        try
        {
            if (_bot.Timeout.Minutes > 1)
            {
                await CloseAndStartConnection();
            }
            await _srvcTelegramBot.Start(bot, upd);
        }
        catch (Exception ex)
        {
            throw new ArgumentException(ex.Message);
        }
    }

    public async Task ExceptionHandler(ITelegramBotClient bot, Exception ex, CancellationToken token)
    {
        if (_bot.Timeout.Minutes > 1)
        {
            await CloseAndStartConnection();
        }
        Start();
    }

    public TelegramBotClient Start()
    {
        try
        {
            _bot = new TelegramBotClient(_config["TelegramToken"]!);
            _bot.StartReceiving(UpdateHandler, ExceptionHandler);
            return _bot;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    private async Task CloseAndStartConnection()
    {
        await _bot.CloseAsync();
        Start();
    }
}
