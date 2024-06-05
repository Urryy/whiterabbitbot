using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class AllUsersHandler : IBaseHandler
{
    public ITelegramBotClient bot;
    public Update upd;
    public Entity.User user;
    public string command;

    public AllUsersHandler(ITelegramBotClient bot, Update upd, Entity.User user, string command)
    {
        this.bot = bot;
        this.upd = upd;
        this.user = user;
        this.command = command;
    }

    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
