using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class EarnWBCoinsHandler : IBaseHandler
{
    public Entity.User user;
    public ITelegramBotClient bot;
    public Update upd;

    public EarnWBCoinsHandler(Entity.User user, ITelegramBotClient bot, Update upd)
    {
        this.user = user;
        this.bot = bot;
        this.upd = upd;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
