using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class ReferralSystemHandler : IBaseHandler
{
    public ITelegramBotClient bot;
    public Update upd;
    public Entity.User user;
    public string textWithRefLink;

    public ReferralSystemHandler(Entity.User user, string textWithRefLink, ITelegramBotClient bot, Update upd)
    {
        this.user = user;
        this.textWithRefLink = textWithRefLink;
        this.bot = bot;
        this.upd = upd;
    }

    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
