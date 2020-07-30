using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.Api.Models
{
    public class MessageUrlDto : MessageDto
    {
        public string URL { get; set; }
    }
}
