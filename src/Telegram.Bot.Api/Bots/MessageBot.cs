using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Framework;
using Telegram.Bot.Types;

namespace Telegram.Bot.Api
{
    public class MessageBot : BotBase
    {
        private readonly ILogger<MessageBot> _logger;
        public MessageBot(IOptions<BotOptions<MessageBot>> botOptions, ILogger<MessageBot> logger)
            : base(botOptions.Value)
        {
            _logger = logger;
        }

    }
}