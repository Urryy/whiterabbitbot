using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class FirstSignHandler : IBaseHandler
{
    public string wallet;
    public Entity.User user;
    public ITelegramBotClient bot;
    public Update upd;

    public FirstSignHandler(string wallet, Entity.User user, ITelegramBotClient bot, Update upd)
    {
        this.wallet= wallet;
        this.user= user;
        this.bot= bot;
        this.upd= upd;
    }

    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
