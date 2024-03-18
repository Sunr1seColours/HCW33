using System.Text.RegularExpressions;
using BotLib;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Requests;
using Telegram.Bot.Types.Enums;

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
    public List<long> Chats { get; }

    public CancellationToken Token => _cts.Token;
    
    public TelegramBotClient BotClient { get; }

    public Bot()
    {
        BotClient = new TelegramBotClient("6996316735:AAG-YMGUcJ3rh2CIFZLgLUtMfqZeksDSBOo");
        _cts = new CancellationTokenSource();
        Chats = new List<long>();
        Updater = new UpdateHandler();
    }
}