using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Framework.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.Api
{
    public class SubscribeHandler : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
        {
            Message msg = context.Update.Message;
            
            var ss = await context.Bot.Client.GetStickerSetAsync("Pokemon");

            int rng = DateTime.Now.Millisecond % ss.Stickers.Length;

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat, $"Who's that Pokemon???"
            );            

            var sticker = ss.Stickers[rng];

            await context.Bot.Client.SendStickerAsync(msg.Chat, sticker.FileId);

            string pokemonName = string.Empty;

            switch (rng)
            {
                case 0:
                    pokemonName = "Bulbasaur";
                    break;
                case 1:
                    pokemonName = "Charmander";
                    break;
                case 2:
                    pokemonName = "Squirtle";
                    break;
                case 3:
                    pokemonName = "Gengar";
                    break;
                case 4:
                    pokemonName = "SlowPoke";
                    break;
                case 5:
                    pokemonName = "Pikachu";
                    break;
                case 6:
                    pokemonName = "PsyDuck";
                    break;
                case 7:
                    pokemonName = "Jiggypuff";
                    break;
                case 8:
                    pokemonName = "Eevee";
                    break;
                case 9:
                    pokemonName = "Snorlax";
                    break;
                case 10:
                    pokemonName = "Dragonite";
                    break;
                case 11:
                    pokemonName = "Primeape";
                    break;
                case 12:
                    pokemonName = "Arcanine";
                    break;
                case 13:
                    pokemonName = "Charizard";
                    break;
                case 14:
                    pokemonName = "Blastoise";
                    break;
                case 15:
                    pokemonName = "MewTwo";
                    break;
                case 16:
                    pokemonName = "Mew";
                    break;
                case 17:
                    pokemonName = "Scyther";
                    break;
                case 18:
                    pokemonName = "Tyranitar";
                    break;
                case 19:
                    pokemonName = "Chikorita";
                    break;
                case 20:
                    pokemonName = "Cyndaquil";
                    break;
                case 21:
                    pokemonName = "Totodile";
                    break;
                case 22:
                    pokemonName = "Ninetails";
                    break;
                case 23:
                    pokemonName = "Koffing";
                    break;
                case 24:
                    pokemonName = "Omanyte";
                    break;
                case 25:
                    pokemonName = "Ho-Oh";
                    break;
                case 26:
                    pokemonName = "Lugia";
                    break;
                case 27:
                    pokemonName = "Articuno";
                    break;
                case 28:
                    pokemonName = "Zapdos";
                    break;
                case 29:
                    pokemonName = "Moltres";
                    break;
                case 30:
                    pokemonName = "Wobbuffet";
                    break;
                case 31:
                    pokemonName = "Pidgeot";
                    break;
                case 32:
                    pokemonName = "Togepi";
                    break;
                case 33:
                    pokemonName = "Ursaring";
                    break;
                case 34:
                    pokemonName = "Ratatta";
                    break;
                case 35:
                    pokemonName = "Star-U";
                    break;
                case 36:
                    pokemonName = "Metapod";
                    break;
               
                default:
                    break;
            }

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat, $"It's {pokemonName}!!!!"
            );

            await next(context);
        }
    }

    public class SubscribeChannelHandler : IUpdateHandler
    {
        public async Task HandleAsync(IUpdateContext context, UpdateDelegate next, CancellationToken cancellationToken)
            {
            Message msg = context.Update.ChannelPost;

            await context.Bot.Client.SendTextMessageAsync(
                msg.Chat, "You said:\n" + msg.Text + $"\n ID: {msg.Chat.Id}"
            );
            await next(context);
        }
    }

}