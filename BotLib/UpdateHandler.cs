using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotLib;

public class UpdateHandler
{
    /// <summary>
    /// Array of commands which bot has.
    /// </summary>
    private string[] _commands = new string[]
    {
        "/start",
        "/help"
    };
    
    /// <summary>
    /// Reacts on '/start' command.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    private async Task StartCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId, "hi", cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Reacts on '/help' command.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    private async Task HelpCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId, "help", cancellationToken: cancellationToken);
    }
    
    public DateTime LastUpdateTime { get; private set; }

    public UpdateHandler()
    {
        LastUpdateTime = DateTime.UtcNow;
    }
    
    public async Task<Update[]> GetNewUpdatesAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {
        Update[] allUpdates = await botClient.GetUpdatesAsync();
        List<Update> newUpdates = new List<Update>();
        foreach (Update update in allUpdates)
        {
            if (update.Message.Date > LastUpdateTime)
            {
                newUpdates.Add(update);
                // if (!botClient.Contains(update.Message.Chat.Id))
                // {
                //     Chats.Add(update.Message.Chat.Id);
                // }
            }
        }

        LastUpdateTime = allUpdates[^1].Message.Date;
        return newUpdates.ToArray();
    }
    
    public async Task<Update[]> GetFileUpdatesAsync(List<Update> updates, Regex pattern, CancellationToken cancellationToken)
    {
        List<Update> fileUpdates = new List<Update>();
        foreach (Update update in updates)
        {
            if (update.Message.Document.FileName != null && pattern.IsMatch(update.Message.Document.FileName))
            {
                fileUpdates.Add(update);
            }
        }

        return fileUpdates.ToArray();
    }

    public async Task CommandHandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        MessageEntity[] entities = update.Message.Entities;
        if (entities is { Length: 1 } && entities[0].Type == MessageEntityType.BotCommand)
        {
            string command = update.Message.Text;
            if (_commands.Contains(command))
            {
                ChatId chatId = update.Message.Chat.Id;
                switch (command)
                {
                    case "/start":
                        await StartCommandHandlerAsync(botClient, chatId, cancellationToken);
                        break;
                    case "/help":
                        await HelpCommandHandlerAsync(botClient,     chatId, cancellationToken);
                        break;
                }
            }
        }
    }
    
}