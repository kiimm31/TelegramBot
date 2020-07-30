using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Framework;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Api.Services
{
    public class KeepAliveService
    {
        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public KeepAliveService(IApplicationLifetime applicationLifetime, IHttpContextAccessor httpContextAccessor)
        {
            this._applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            _httpContextAccessor = httpContextAccessor;
        }

        internal Task DoRestart(ITelegramBotClient client, string debugGroupId, AggregateException exception, string siteName)
        {
            Task.Run(() => client.SendTextMessageAsync(debugGroupId,
   $"Update Manager has thrown an exception:\n" +
   exception.GetBaseException().Message,
    Types.Enums.ParseMode.Markdown,
    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
        "Click to restart", $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{siteName}"))));

            _applicationLifetime.StopApplication();
            return Task.CompletedTask;

            
        }
    }
}
