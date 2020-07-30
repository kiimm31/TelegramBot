using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;

namespace Telegram.Bot.Api.Handlers.Exceptions
{
    public class ExceptionHandler : IUpdateHandler
    {
        private readonly string _debugGroupId;

        public ExceptionHandler(IConfiguration configuration)
        {
            _debugGroupId = configuration.GetValue<string>("DebugGroup", "-368260554");
        }

        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Types.Update u = context.Update;
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("An error occured in handling update {0}.{1}{2}", u.Id, Environment.NewLine, e);
                Console.ResetColor();
                await context.Bot.Client.SendTextMessageAsync(
    chatId: _debugGroupId, string.Format("An error occured in handling update {0}.{1}{2}", u.Id, Environment.NewLine, e)
);

            }
        }
    }
}
