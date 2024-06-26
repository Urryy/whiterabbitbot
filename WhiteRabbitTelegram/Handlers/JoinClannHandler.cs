﻿using Telegram.Bot;
using Telegram.Bot.Types;
using WhiteRabbitTelegram.Handlers.Interface;
using WhiteRabbitTelegram.Visitor;

namespace WhiteRabbitTelegram.Handlers;

public class JoinClannHandler : IBaseHandler
{
    public Entity.User user { get; set; }
    public ITelegramBotClient bot { get; set; }
    public Update upd { get; set; }
    public string text { get; set; }

    public JoinClannHandler(Entity.User user, ITelegramBotClient bot, Update upd, string text)
    {
        this.user = user;
        this.bot = bot;
        this.upd = upd;
        this.text = text;
    }
    public async Task Accept(IBaseVisitor visitor)
    {
        await visitor.Visit(this);
    }
}
