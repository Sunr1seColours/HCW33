using BotLib;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace HCW33;

/// <summary>
/// Releases all bot logic.
/// </summary>
public class Bot
{
    /// <summary>
    /// Private pole which contains offset of last update.
    /// </summary>
    private int _off = 0;
    
    /// <summary>
    /// Cancellation Token Source.
    /// </summary>
    private CancellationTokenSource _cts;
    
    /// <summary>
    /// Property which contains object to handle updates from bot.
    /// </summary>
    public UpdateHandler Updater { get; init; }
    
    /// <summary>
    /// Dictionary of pairs, where key is chatId and value is UserInfo object. 
    /// </summary>
    public Dictionary<long, UserInfo> Chats { get; }

    /// <summary>
    /// Cancellation token.
    /// </summary>
    public CancellationToken Token => _cts.Token;
    
    /// <summary>
    /// Client of bot. Needs for sending messages.
    /// </summary>
    public TelegramBotClient BotClient { get; }

    /// <summary>
    /// Base bot constructor with no parameters.
    /// </summary>
    public Bot()
    {
        // Enter your bot's token here.
        BotClient = new TelegramBotClient("");
        _cts = new CancellationTokenSource();
        Chats = new Dictionary<long, UserInfo>();
        Updater = new UpdateHandler();
    }

    /// <summary>
    /// Gets new updates.
    /// </summary>
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

        if (allUpdates.Length <= 0) return newUpdates.ToArray();
        if (allUpdates[^1].Message != null)
        {
            Updater.LastUpdateTime = allUpdates[^1].Message.Date;    
        }
        _off = allUpdates[^1].Id;
        return newUpdates.ToArray();
    }
}