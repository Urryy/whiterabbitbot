using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class CreateClannHandler : IBaseHandler
{
    public Entity.User user { get; set; }
    public ITelegramBotClient bot { get; set; }
    public Update upd { get; set; }

    public CreateClannHandler(Entity.User user, ITelegramBotClient bot, Update upd)
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
