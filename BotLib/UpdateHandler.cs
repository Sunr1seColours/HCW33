using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Requests;
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

    private async Task UploadChoiceAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        CancellationToken cancellationToken)
    {
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Загрузи файл", showAlert: true,
            cancellationToken: cancellationToken);
    }
    
    private async Task ActionChoiceAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserInfo user,
        CancellationToken cancellationToken)
    {
        user.State = string.Equals(callbackQuery.Data, "Выборка") ?
            UserInfo.UserStates.ChoosingAttributeForSelection : UserInfo.UserStates.ChoosingAttributeForSorting;
        long chatId = callbackQuery.From.Id;
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, "Выбери поле выборки",
            cancellationToken: cancellationToken);
        
        if (user.State == UserInfo.UserStates.ChoosingAttributeForSelection)
        {
            await AskSelectionParameterAsync(botClient, chatId, user, cancellationToken);
        }
        else
        {
            await AskSortingParameterAsync(botClient, chatId, user, cancellationToken);
        }

        user.State = UserInfo.UserStates.WaitingForCallback;
    }

    private async Task SelectAttributeAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserInfo user, CancellationToken cancellationToken)
    {
        long chatId = callbackQuery.From.Id;
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, $"Выборка будет сделана по парметру {callbackQuery.Data}",
            showAlert: true, cancellationToken: cancellationToken);
        await botClient.SendTextMessageAsync(chatId,
            $"Введи значение {callbackQuery.Data}, по которому будет сделана выборка");
        user.State = UserInfo.UserStates.EnteringValueForSelection;
    }

    private async Task SortingAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserInfo user,
        CancellationToken cancellationToken)
    {
        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, 
            $"Сортировка будет выполнена по {callbackQuery.Data.ToLower()}",
            showAlert: true, cancellationToken: cancellationToken);
        user.State = UserInfo.UserStates.None;
    }
    
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
                                                     "1. AdmArea\n" +
                                                     "2. District\n" +
                                                     "3. AdmArea + Coordinates", replyMarkup: keyboard,
            cancellationToken: cancellationToken);
        user.State = UserInfo.UserStates.WaitingForCallback;
    }

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
                        "Alphabetical"
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        "2",
                        "Reversed" 
                    ),
                }
            });
        await botClient.SendTextMessageAsync(chatId, "Выбери поле для сортировки:\n" +
                                                     "1. AdmArea по алфавиту\n" +
                                                     "2. AdmArea по алфавиту в обратном порядке\n", replyMarkup: keyboard,
            cancellationToken: cancellationToken);
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
                        user.State = UserInfo.UserStates.UploadingFiles;
                        break;
                }
            }
        }
    }

    public async Task UploadingFileHandlerAsync(ITelegramBotClient botClient, Update update, UserInfo user,
        CancellationToken cancellationToken)
    {
        string fileName = update.Message.Document.FileName;
        Regex csv = new Regex(".csv$");
        Regex json = new Regex(".json$");
        if (csv.IsMatch(fileName) || json.IsMatch(fileName))
        {
            using (Stream file = System.IO.File.Create($"../../../../receivedFiles/{fileName}"))
            {
                await botClient.GetInfoAndDownloadFileAsync(update.Message.Document.FileId, file, cancellationToken);
                user.File = file;
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
    
    public async Task CallbackAnswerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserInfo user, CancellationToken cancellationToken)
    {
        switch (callbackQuery.Data)
        {
            case "Новый файл":
                await UploadChoiceAnswerAsync(botClient, callbackQuery, cancellationToken);
                user.State = UserInfo.UserStates.UploadingFiles;
                break;
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
                await SelectAttributeAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "Alphabetical":
                user.TypeOfAction = 4;
                await SortingAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
            case "Reversed":
                user.TypeOfAction = 5;
                await SortingAnswerAsync(botClient, callbackQuery, user, cancellationToken);
                break;
        }
    }
}