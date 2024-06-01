using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Records;

namespace WhiteRabbitTelegram.Visitor
{
    public interface IBaseVisitor
    {
        Task Visit(CheckNFTCollectionHandler handler);
        Task Visit(CheckWhiteCoinsHandler handler);
        Task Visit(EarnWBCoinsHandler handler);
        Task Visit(TopUsersHandler handler);
        Task Visit(PersonalAccountHandler handler);
        Task Visit(ReferralSystemHandler handler);
    }
}
