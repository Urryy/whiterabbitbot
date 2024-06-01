using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class CheckWhiteCoinsHandler : IBaseHandler
{
    public readonly ITelegramBotClient _bot;
    public readonly Update _upd;
    public readonly string _wallet;
    public Entity.User user;

    public CheckWhiteCoinsHandler(ITelegramBotClient bot, Update upd, string wallet, Entity.User user)
    {
        _bot = bot;
        _wallet = wallet;
        _upd = upd;
        this.user = user;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
