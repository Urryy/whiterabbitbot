using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TonSdk.Client;
using TonSdk.Core.Block;

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

    //public static async Task SendMessage(this ITelegramBotClient bot, Update upd, Entity.User user, string text, InlineKeyboardMarkup buttons = null) 
    //{
    //    var chatId = await upd.GetChatId();
    //    if (upd.Type == UpdateType.CallbackQuery)
    //    {
    //        if(user.LastMessageId != null)
    //        {
    //            if (buttons != null)
    //            {
    //                await bot.EditMessageTextAsync(user.LastMessageId, text, replyMarkup: buttons);
    //            }
    //            else
    //            {
    //                await bot.EditMessageTextAsync(user.LastMessageId, text);
    //            }
    //        }
    //        else
    //        {
    //            if (buttons != null)
    //            {
    //                await bot.SendTextMessageAsync(chatId, text, replyMarkup: buttons);
    //            }
    //            else
    //            {
    //                await bot.SendTextMessageAsync(chatId, text);
    //            }
    //        }
    //        await bot.EditMessageTextAsync(chatId, chatId, "This message has been edited!");
    //        user.LastMessageId = upd.CallbackQuery.InlineMessageId;
    //    }
    //    else
    //    {
    //        if (buttons != null)
    //        {
    //            await bot.SendTextMessageAsync(chatId, text, replyMarkup: buttons);
    //        }
    //        else
    //        {
    //            await bot.SendTextMessageAsync(chatId, text);
    //        }
    //        user.LastMessageId = null;
    //    }
    //}

    public static async Task SendMessage(this ITelegramBotClient bot, Update upd, Entity.User user, string text, bool isEdit, InlineKeyboardMarkup buttons = null)
    {
        var chatId = await upd.GetChatId();
        if(user.LastMessageId != null && isEdit)
        {
            if (buttons != null)
            {
                await bot.EditMessageTextAsync(chatId, user.LastMessageId.Value, text, replyMarkup: buttons);
            }
            else
            {
                await bot.EditMessageTextAsync(chatId, user.LastMessageId.Value, text);
            }
        }
        else
        {
            if (buttons != null)
            {
                user.LastMessageId = (await bot.SendTextMessageAsync(chatId, text, replyMarkup: buttons)).MessageId;
            }
            else
            {
                user.LastMessageId = (await bot.SendTextMessageAsync(chatId, text)).MessageId;
            }
        }
    }
}
