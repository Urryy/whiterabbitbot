using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace WhiteRabbitTelegram.Extension;

public static class ChatExtension
{
    public static async Task<long> GetChatId(this Update upd)
    {
        if(upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery.Message.Chat.Id;
        }
        else if(upd.Type == UpdateType.Message)
        {
            return upd.Message.Chat.Id;
        }
        else
        {
            return 0;
        }
    }

    public static async Task<string> GetText(this Update upd)
    {
        if(upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery.Data;
        }
        else if(upd.Type == UpdateType.Message)
        {
            return upd.Message.Text;
        }
        else
        {
            return string.Empty;
        }
    }

    public static async Task<string> GetCallbackQueryId(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery.Id;
        }
        return string.Empty;
    }

    public static async Task<bool> IsTelegramPremium(this Update upd)
    {
        if (upd.Type == UpdateType.CallbackQuery)
        {
            return upd.CallbackQuery.From.IsPremium ?? false;
        }
        else if (upd.Type == UpdateType.Message)
        {
            
            return upd.Message!.From!.IsPremium ?? false;
        }
        else
        {
            return false;
        }
    }
}
