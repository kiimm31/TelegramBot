using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Api.Extensions;
using Telegram.Bot.Api.Models;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Telegram.Bot.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotSettings _settings;
        private readonly TelegramBotClient _botClient;

        public BotController(IOptions<BotSettings> settings)
        {
            _settings = settings == null
                ? throw new ArgumentNullException()
                : _settings = settings.Value;
            _botClient = new TelegramBotClient(_settings.ApiToken);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageDto message)
        {
            if (!FunctionalExtensions.Check<MessageDto>(message, x => x.ChatId != null || x.MessageText != null || x.Sender != null))
            {
                return BadRequest("All fields are necessary");
            }
            try
            {
                Types.Message outMsg = await _botClient.SendTextMessageAsync(
    chatId: message.ChatId,
    text: $"{message.Sender} : {message.MessageText}",
    parseMode: Types.Enums.ParseMode.Markdown
    );

                return Ok(outMsg);
            }
            catch (ApiRequestException ex)
            {

                Console.WriteLine($"Error code:{ex.ErrorCode}");
                return BadRequest(ex);
            }

        }

        [HttpPost("PostLinkUrlHtml")]
        public async Task<IActionResult> SendMessageWithUrlHtml(MessageUrlDto message)
        {
            if (!FunctionalExtensions.Check<MessageUrlDto>(message, x => x.ChatId != null || x.MessageText != null || x.URL != null))
            {
                return BadRequest("All fields are necessary");
            }
            try
            {
                Types.Message outMsg = await _botClient.SendTextMessageAsync(message.ChatId,
    message.MessageText,
    Types.Enums.ParseMode.Html,
    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
        "Action link", message.URL)));

                return Ok(outMsg);
            }
            catch (ApiRequestException ex)
            {

                Console.WriteLine($"Error code:{ex.ErrorCode}");
                return BadRequest(ex);
            }
        }

        [HttpPost("PostLinkUrlMd")]
        public async Task<IActionResult> SendMessageWithUrlMarkdown(MessageUrlDto message)
        {
            if (!FunctionalExtensions.Check<MessageUrlDto>(message, x => x.ChatId != null || x.MessageText != null || x.URL != null))
            {
                return BadRequest("All fields are necessary");
            }
            try
            {
                Types.Message outMsg = await _botClient.SendTextMessageAsync(message.ChatId,
    message.MessageText,
    Types.Enums.ParseMode.Markdown,
    replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
        "Action link", message.URL)));

                return Ok(outMsg);
            }
            catch (ApiRequestException ex)
            {

                Console.WriteLine($"Error code:{ex.ErrorCode}");
                return BadRequest(ex);
            }
        }
        [HttpPost("GetChatGroup")]
        public async Task<IActionResult> GetChatGroup([FromQuery]string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest($"{nameof(groupName)} is required");
            }
            Types.Update[] updates = await _botClient.GetUpdatesAsync();
            if (updates.Any())
            {

                foreach (Types.Update item in updates)
                {
                    if (item.Message != null)
                    {
                        if (item.Message.Chat.Type == Types.Enums.ChatType.Group && item.Message.Chat.Title == groupName && item.Message.Text == "arcstone subscribe")
                        {
                            await _botClient.SendTextMessageAsync(item.Message.Text,
                                "Create A Web Api notification using the returned Chat Id\n" +
                                "Chat Id: " + item.Message.Chat.Id);
                            return Ok(new { ChatId = item.Message.Chat.Id, GroupName = item.Message.Chat.Title });
                        }
                    }
                }
            }
            return NotFound();
        }


    }
}