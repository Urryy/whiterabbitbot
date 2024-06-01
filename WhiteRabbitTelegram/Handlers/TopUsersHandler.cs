using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Command;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class TopUsersHandler : IBaseHandler
{
    public Update upd;
    public ITelegramBotClient bot;
    public Entity.User user;
    public string command;

    public TopUsersHandler(Entity.User user, ITelegramBotClient bot, Update upd, string command)
    {
        this.user = user;
        this.bot = bot;
        this.upd = upd;
        this.command = command;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
