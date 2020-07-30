using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Api.Handlers.Commands
{
    class StartCommand : CommandBase
    {
        public async override Task HandleAsync(IUpdateContext context, UpdateDelegate next, string[] args, CancellationToken cancellationToken)
        {
            await context.Bot.Client.SendTextMessageAsync(context.Update.Message.Chat, 
                "Usage:\n" +
                "Add Bot to group, and create Web Api notification using the returned Chat Id\n" +
                "Chat Id: " + context.Update.Message.Chat.Id,
                Types.Enums.ParseMode.Markdown);
            await next(context);
        }
    }
}
