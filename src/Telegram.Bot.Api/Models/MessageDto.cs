using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.Api.Models
{
    public class MessageDto
    {
        public string ChatId { get; set; }

        public string MessageText { get; set; }

        public string Sender { get; set; }
    }
}
