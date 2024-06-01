using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers.Interface;

public interface IBaseHandler
{
    Task Accept(IBaseVisitor visitor);
}
