using Microsoft.Identity.Client;
using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class PersonalAccountHandler : IBaseHandler
{
    public ITelegramBotClient bot;
    public Update upd;
    public Entity.User user;
    public PersonalAccountHandler(ITelegramBotClient bot, Update upd, Entity.User user)
    {
        this.bot = bot;
        this.upd = upd;
        this.user = user;
    }

    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
