using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;

namespace HCW33;

public class Bot
{
    private CancellationTokenSource _cts;
    
    private string[] _commands = new string[]
    {
        "/start",
        "/help"
    };

    private async Task StartCommandHandlerAsync(ChatId chatId, CancellationToken token)
    {
        // await BotClient.SendTextMessageAsync(chatId);
    }

    private async Task HelpCommandHandlerAsync(CancellationToken token)
    {
        
    }
    
    public List<long> Chats { get; }

    public DateTime StartTime { get; private set; }

    public CancellationToken Token => _cts.Token;
    
    public TelegramBotClient BotClient { get; }

    public Bot()
    {
        BotClient = new TelegramBotClient("your bot token");
        _cts = new CancellationTokenSource();
        Chats = new List<long>();
        StartTime = DateTime.UtcNow;
    }
    
    public async Task<Update[]> GetNewUpdatesAsync(CancellationToken token)
    {
        Update[] allUpdates = await BotClient.GetUpdatesAsync();
        List<Update> newUpdates = new List<Update>();
        foreach (Update update in allUpdates)
        {
            if (update.Message.Date > StartTime)
            {
                newUpdates.Add(update);
                if (!Chats.Contains(update.Message.Chat.Id))
                {
                    Chats.Add(update.Message.Chat.Id);
                }
            }
        }

        StartTime = allUpdates[^1].Message.Date;
        return newUpdates.ToArray();
    }

    public async Task<Update[]> GetFileUpdatesAsync(List<Update> updates, Regex pattern, CancellationToken token)
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

    public async Task CommandHandlerAsync(Update update, CancellationToken token)
    {
        MessageEntity[] entities = update.Message.Entities;
        if (entities.Length == 1 && entities[0].Type == MessageEntityType.BotCommand)
        {
            string command = entities[0].ToString();
            if (_commands.Contains(command))
            {
                switch (command)
                {
                    case "/start":
                        break;
                    case "/help":
                        break;
                }
            }
        }
    }
}