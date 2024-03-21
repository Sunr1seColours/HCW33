using System.Text.RegularExpressions;
using BotLib;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;
using File = Telegram.Bot.Types.File;

namespace HCW33;

/// <summary>
/// Releases all bot logic.
/// </summary>
public class Bot
{
    /// <summary>
    /// Cancellation Token Source.
    /// </summary>
    private CancellationTokenSource _cts;
    
    public UpdateHandler Updater { get; init; }
    public Dictionary<long, UserInfo> Chats { get; }

    public CancellationToken Token => _cts.Token;
    
    public TelegramBotClient BotClient { get; }

    private int _off = 0;

    public Bot()
    {
        BotClient = new TelegramBotClient("6996316735:AAG-YMGUcJ3rh2CIFZLgLUtMfqZeksDSBOo");
        _cts = new CancellationTokenSource();
        Chats = new Dictionary<long, UserInfo>();
        Updater = new UpdateHandler();
    }
    
    /// <summary>
    /// Gets new updates.
    /// </summary>
    /// <param name="botClient">Bot which gets updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Array of new updates.</returns>
    public async Task<Update[]> GetNewUpdatesAsync(CancellationToken cancellationToken)
    {
        Update[] allUpdates = await BotClient.GetUpdatesAsync(offset: _off, cancellationToken: cancellationToken);
        List<Update> newUpdates = new List<Update>();
        foreach (Update update in allUpdates)
        {
            if (update.Message != null && update.Message.Date > Updater.LastUpdateTime)
            {
                newUpdates.Add(update);
                if (!Chats.Keys.Contains(update.Message.Chat.Id))
                {
                    Chats.Add(update.Message.Chat.Id, new UserInfo());
                }
            }
            else if (update.CallbackQuery != null)
            {
                if (Chats.Keys.Count != 0)
                {
                    newUpdates.Add(update);    
                }
            }
        }

        if (allUpdates.Length > 0)
        {
            if (allUpdates[^1].Message != null)
            {
                Updater.LastUpdateTime = allUpdates[^1].Message.Date;    
            }
            _off = allUpdates[^1].Id;
        }
        return newUpdates.ToArray();
    }

    
}