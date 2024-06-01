using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Records;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class CheckNFTCollectionHandler : IBaseHandler
{
    public readonly ITelegramBotClient _bot;
    public readonly Update _upd;
    public Entity.User user;
    public string wallet;

    public CheckNFTCollectionHandler(ITelegramBotClient bot, Update upd, Entity.User user, string wallet)
    {
        _bot = bot;
        _upd = upd;
        this.user = user;
        this.wallet = wallet;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
