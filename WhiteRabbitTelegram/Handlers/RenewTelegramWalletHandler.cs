using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class RenewTelegramWalletHandler : IBaseHandler
{
    public ITelegramBotClient bot;
    public Update upd;
    public Entity.User user;
    public string wallet;

    public RenewTelegramWalletHandler(Entity.User user, ITelegramBotClient bot, Update upd, string wallet)
    {
        this.user = user;
        this.bot = bot;
        this.upd = upd;
        this.wallet = wallet;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
