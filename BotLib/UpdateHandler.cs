using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using File = Telegram.Bot.Types.File;

namespace BotLib;

/// <summary>
/// Class which contains methods to communicate with user.
/// </summary>
public class UpdateHandler
{
    /// <summary>
    /// Array of commands which bot has.
    /// </summary>
    private string[] _commands = new string[]
    {
        "/start",
        "/help",
        "/make_action",
        "/restart"
    };
    
    /// <summary>
    /// Reacts on '/start' command.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where reaction is needed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task StartCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
    {
        await botClient.SendStickerAsync(chatId, new InputFileId("CAACAgIAAxkBAAELwgpl_AABdda70h1W5DsYME1JI_gPUToAAgEBAAJWnb0KIr6fDrjC5jQ0BA"),
            cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId, "Привет\\! Это бот, который поможет работать с *csv* и *json* файлами," +
                                                     " содержащими информацию о зарядках для электромобилей\\.", 
            parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Reacts on '/help' command.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where reaction is needed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task HelpCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId, CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId, "Напиши /make_action для начала работы с ботом\n" +
                                                     "Используй /restart, если что-то пошло не по плану - перезапуск работы с файлом",
            cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Reacts on '/make_action' command.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where reaction is needed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task MakeActionCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId, "Загрузи файл", cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Reacts on "/restart" command.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where reaction is needed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task RestartCommandHandlerAsync(ITelegramBotClient botClient, ChatId chatId,
        CancellationToken cancellationToken)
    {
        await botClient.SendTextMessageAsync(chatId,
            "Работа с файлом прекращена, чтобы ее начать снова, используй команду /make_action",
            cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Reacts on callback when user decide what action he want.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="callbackQuery">Information about callback.</param>
    /// <param name="user">UserInfo object which represents user that needs answer for callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task ActionChoiceAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserInfo user,
        CancellationToken cancellationToken)
    {
        user.State = string.Equals(callbackQuery.Data, "Выборка") ?
            UserInfo.UserStates.ChoosingAttributeForSelection : UserInfo.UserStates.ChoosingAttributeForSorting;
        long chatId = callbackQuery.From.Id;
        if (user.State == UserInfo.UserStates.ChoosingAttributeForSelection)
        {
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Выбери поле выборки",
                cancellationToken: cancellationToken);
            await AskSelectionParameterAsync(botClient, chatId, user, cancellationToken);
        }
        else
        {
            await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Выбери порядок сортировки",
                cancellationToken: cancellationToken);
            await AskSortingParameterAsync(botClient, chatId, user, cancellationToken);
        }

        user.State = UserInfo.UserStates.WaitingForCallback;
    }

    /// <summary>
    /// Reacts on callback when user chose parameter for selection. Also asks user for value of this parameter.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="callbackQuery">Information about callback.</param>
    /// <param name="user">UserInfo object which represents user that needs answer for callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SelectAttributeAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserInfo user, CancellationToken cancellationToken)
    {
        long chatId = callbackQuery.From.Id;
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"Выборка будет сделана по парметру {callbackQuery.Data}", 
            cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId,
            $"Введи значение {callbackQuery.Data}, по которому будет сделана выборка");
        user.State = UserInfo.UserStates.EnteringValueForSelection;
    }
    
    /// <summary>
    /// Reacts on callback when user chose make selection by some parameters. Also asks user for value of them.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="callbackQuery">Information about callback.</param>
    /// <param name="user">UserInfo object which represents user that needs answer for callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SelectSomeAttributeAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserInfo user, CancellationToken cancellationToken)
    {
        long chatId = callbackQuery.From.Id;
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"Выборка будет сделана по парметрам {callbackQuery.Data}",
            cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId,
            $"Введи значения через *\\|* AdmArea, Latitude, Longitude, по которым будет сделана выборка",
            parseMode: ParseMode.MarkdownV2);
        user.State = UserInfo.UserStates.EnteringValueForSelection;
    }

    /// <summary>
    /// Tells user order of sorting when user chose it.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="callbackQuery">Information about callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task SortingAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, 
            $"Сортировка будет выполнена {callbackQuery.Data.ToLower()}",
            showAlert: true, cancellationToken: cancellationToken);
    }
    
    /// <summary>
    /// Asks user by what parameter selection is needed.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where question is needed.</param>
    /// <param name="user">Information about user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task AskSelectionParameterAsync(ITelegramBotClient botClient, ChatId chatId, UserInfo user,
        CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
            new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "1",
                        "AdmArea"
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        "2",
                        "District" 
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        "3",
                        "DifficultAdmArea" 
                    ),
                }
            });
        await botClient.SendTextMessageAsync(chatId, "Выбери поле для выборки:\n" +
                                                     "1. Административный округ\n" +
                                                     "2. Район\n" +
                                                     "3. Административный округ и координаты", replyMarkup: keyboard,
            cancellationToken: cancellationToken);
        user.State = UserInfo.UserStates.WaitingForCallback;
    }

    /// <summary>
    /// Asks user in which order sorting is needed.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where question is needed.</param>
    /// <param name="user">Information about user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    private async Task AskSortingParameterAsync(ITelegramBotClient botClient, ChatId chatId, UserInfo user,
        CancellationToken cancellationToken)
    {
        InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
            new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        "1",
                        "В алфавитном порядке"
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        "2",
                        "В обратном алфавитном порядке" 
                    ),
                }
            });
        await botClient.SendTextMessageAsync(chatId, "Выбери параметр сортировки:\n" +
                                                     "1. Административный округ по алфавиту\n" +
                                                     "2. Административный округ по алфавиту в обратном порядке\n",
            replyMarkup: keyboard, cancellationToken: cancellationToken);
        user.State = UserInfo.UserStates.WaitingForCallback;
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

    /// <summary>
    /// Reacts on message with command.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="update">Information about update.</param>
    /// <param name="user">Information about user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CommandHandlerAsync(ITelegramBotClient botClient, Update update, UserInfo user, CancellationToken cancellationToken)
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
                        user.State = UserInfo.UserStates.UploadingFile;
                        break;
                    case "/restart":
                        await RestartCommandHandlerAsync(botClient, chatId, cancellationToken);
                        user.State = UserInfo.UserStates.None;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Reacts on message with file.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="update">Information about update.</param>
    /// <param name="user">Information about user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task UploadingFileHandlerAsync(ITelegramBotClient botClient, Update update, UserInfo user,
        CancellationToken cancellationToken)
    {
        string fileName = update.Message.Document.FileName;
        Regex csv = new Regex(".csv$");
        Regex json = new Regex(".json$");
        if (csv.IsMatch(fileName) || json.IsMatch(fileName))
        {
            user.IsCsv = csv.IsMatch(fileName);
            char sep = Path.DirectorySeparatorChar;
            await using (Stream file = System.IO.File.Open($"..{sep}..{sep}..{sep}..{sep}receivedFiles{sep}{fileName}",
                             FileMode.OpenOrCreate))
            {
                await botClient.GetInfoAndDownloadFileAsync(update.Message.Document.FileId, file, cancellationToken);
                user.File = $"..{sep}..{sep}..{sep}..{sep}receivedFiles{sep}{fileName}";
            }
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Файл получен успешно");
            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(
                new InlineKeyboardButton[][]
                {
                    new InlineKeyboardButton[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            "1",
                            "Выборка" 
                        ),
                        InlineKeyboardButton.WithCallbackData(
                            "2",
                            "Сортировка" 
                        ),
                    }
                });
            await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Выбери, что делать дальше:\n" +
                "1. Провести выборку по одному из полей файла\n" +
                "2. Отсортировать файл по одному из полей файла",
                replyMarkup: keyboard);
            user.State = UserInfo.UserStates.WaitingForCallback;
            return;
           
        } 
        await botClient.SendTextMessageAsync(update.Message.Chat.Id, 
            "Расширение файла должно быть csv или json. Попробуй еще раз");
    }
    
    /// <summary>
    /// Reacts on tapping button in inline keyboard.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="callbackQuery">Information about update.</param>
    /// <param name="user">UserInfo object which represents user that needs answer for callback.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CallbackAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserInfo user,
        CancellationToken cancellationToken)
    {
        switch (callbackQuery.Data)
        {
            case "Выборка":
            case "Сортировка":
                await ActionChoiceAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "AdmArea":
                user.TypeOfAction = 1;
                await SelectAttributeAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "District":
                user.TypeOfAction = 2;
                await SelectAttributeAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "DifficultAdmArea":
                user.TypeOfAction = 3;
                await SelectSomeAttributeAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "В алфавитном порядке":
                user.TypeOfAction = 4;
                await SortingAnswerAsync(botClient, callbackQuery, cancellationToken);
                break;
            case "В обратном алфавитном порядке":
                user.TypeOfAction = 5;
                await SortingAnswerAsync(botClient, callbackQuery, cancellationToken);
                break;
        }
    }

    /// <summary>
    /// Sends file to user.
    /// </summary>
    /// <param name="botClient">Client of bot.</param>
    /// <param name="chatId">Chat ID where to send file.</param>
    /// <param name="fileToSend">File to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="name">Name which file will has.</param>
    public async Task SendFileAsync(ITelegramBotClient botClient, ChatId chatId, Stream fileToSend,
        CancellationToken cancellationToken, string? name = null)
    {
        InputFile file = new InputFileStream(fileToSend, name);
        await botClient.SendDocumentAsync(chatId, file, cancellationToken: cancellationToken);
        fileToSend.Close();
    }
}