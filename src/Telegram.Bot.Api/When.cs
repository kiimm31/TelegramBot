﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.Enums;
namespace Telegram.Bot.Api
{
    public static class When
    {
        public static bool CorrectPokemon(IUpdateContext context) =>
            context.Update.Message?.Text.ToLower() == "/getpokemon";
        public static bool ChannelPost(IUpdateContext context)
            => context.Update.ChannelPost?.Text.ToLower() == "/random";
        public static bool Webhook(IUpdateContext context)
            => context.Items.ContainsKey(nameof(HttpContext));
        public static bool NewMessage(IUpdateContext context) =>
            context.Update.Message != null;
        public static bool NewTextMessage(IUpdateContext context) =>
            context.Update.Message?.Text == null;
        public static bool NewCommand(IUpdateContext context) =>
            context.Update.Message?.Entities?.FirstOrDefault()?.Type == MessageEntityType.BotCommand;
        public static bool MembersChanged(IUpdateContext context) =>
            context.Update.Message?.NewChatMembers != null ||
            context.Update.Message?.LeftChatMember != null ||
            context.Update.ChannelPost?.NewChatMembers != null ||
            context.Update.ChannelPost?.LeftChatMember != null
        ;
        public static bool LocationMessage(IUpdateContext context) =>
            context.Update.Message?.Location != null;
        public static bool StickerMessage(IUpdateContext context) =>
            context.Update.Message?.Sticker != null;
        public static bool CallbackQuery(IUpdateContext context) =>
            context.Update.CallbackQuery != null;
    }
}
