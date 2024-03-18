using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BotLib;

public class UpdateHandler
{
    /// <summary>
    /// Array of commands which bot has.
    /// </summary>
    private string[] _commands = new string[]
    {
        "/start",
        "/help",
        "/make_action"
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
        await botClient.SendTextMessageAsync(chatId, "Напиши /make_action для начала работы с ботом",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Reacts on '/make_action' command.
    /// </summary>
    /// <param name="botClient"></param>
    /// <param name="chatId"></param>
    /// <param name="cancellationToken"></param>
    private async Task MakeActionCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId, "Загрузи файл", cancellationToken: cancellationToken);
    }
    /// <summary>
    /// Property which shows when last update was.
    /// </summary>
    public DateTime LastUpdateTime { get; set; }

    /// <summary>
    /// Constructor with no parameters. Sets time of last update.
    /// </summary>
    public UpdateHandler()
    {
        LastUpdateTime = DateTime.UtcNow;
    }

    public async Task<long> CommandHandlerAsync(ITelegramBotClient botClient, Update update, UserInfo user, CancellationToken cancellationToken)
    {
        MessageEntity[] entities = update.Message.Entities;
        if (entities is { Length: 1 } && entities[0].Type == MessageEntityType.BotCommand)
        {
            string command = update.Message.Text;
            if (_commands.Contains(command))
            {
                long chatId = update.Message.Chat.Id;
                switch (command)
                {
                    case "/start":
                        await StartCommandHandlerAsync(botClient, chatId, cancellationToken);
                        break;
                    case "/help":
                        await HelpCommandHandlerAsync(botClient, chatId, cancellationToken);
                        break;
                    case "/make_action":
                        await MakeActionCommandHandlerAsync(botClient, chatId, cancellationToken);
                        user.State = UserInfo.UserStates.UploadingFiles;
                        return chatId;
                }
            }
        }

        return 0;
    }

    public async Task FileHandlerAsync(ITelegramBotClient botClient, Update update, UserInfo user,
        CancellationToken cancellationToken)
    {
        if (update.Message != null)
        {
            string fileName = update.Message.Document.FileName;
            Regex csv = new Regex(".csv$");
            Regex json = new Regex(".json$");
            if (csv.IsMatch(fileName) || json.IsMatch(fileName))
            {
                using (Stream file = System.IO.File.Create(fileName))
                {
                    await botClient.GetInfoAndDownloadFileAsync(update.Message.Document.FileId, file, cancellationToken);
                    user.Files.Add(file);
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Файл получен успешно");
                    user.State = UserInfo.UserStates.InAction;
                    InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
                        new InlineKeyboardButton[][]
                        {
                            new InlineKeyboardButton[]
                            {
                                InlineKeyboardButton.WithCallbackData(
                                    "1",
                                    "FileHandlerAsync"
                                ),
                                InlineKeyboardButton.WithCallbackData(
                                    "2",
                                    "2" 
                                ),
                                InlineKeyboardButton.WithCallbackData(
                                    "3",
                                    "3" 
                                ),
                                InlineKeyboardButton.WithCallbackData(
                                    "4",
                                    "4" 
                                ),
                            }
                        });
                    await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Выбери, что делать дальше:\n" +
                        "1. Загрузить Csv файл\n" +
                        "2. Загрузить Json файл\n" +
                        "3. Провести выборку по одному из полей файла\n" +
                        "4. Отсортировать файл по одному из полей файла",
                        replyMarkup: keyboard);
                    return;
                }   
            } 
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, 
                "Расширение файла должно быть csv или json. Попробуй еще раз");   
        }
        
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
}