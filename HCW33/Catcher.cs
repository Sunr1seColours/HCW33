using BotLib;
using CsvHelper;
using ElectricChargesLib;
using FileOpsLib;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace HCW33;

/// <summary>
/// Static class which releases all program logic and catches exceptions.
/// </summary>
public static class Catcher
{
    /// <summary>
    /// Restarts operation process on file.
    /// </summary>
    /// <param name="bot">Bot object.</param>
    /// <param name="update">Update from user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if restart is needed. False in another case.</returns>
    private static async Task<bool> Restart(Bot bot, Update update, CancellationToken cancellationToken)
    {
        MessageEntity[] entities = update.Message.Entities;
        if (entities is { Length: 1 } && entities[0].Type == MessageEntityType.BotCommand)
        {
            if (string.Equals(update.Message.Text, "/restart"))
            {
                try
                {
                    await bot.Updater.CommandHandlerAsync(bot.BotClient, update, bot.Chats[update.Message.Chat.Id],
                        cancellationToken);
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    bot.Chats[update.Message.Chat.Id].State = UserInfo.UserStates.None;
                }
            }
        }

        return false;
    }
    
    /// <summary>
    /// Makes operations on file.
    /// </summary>
    /// <param name="stream">Stream object which represents file.</param>
    /// <param name="user">Information about user.</param>
    /// <returns>Array of Stream objects which user will receive.</returns>
    private static Stream[] HandleFile(Stream stream, UserInfo user)
    {
        IProcessing[] processes = { new CSVProcessing(), new JSONProcessing() };
        IProcessing process;
        if (user.IsCsv != null && (bool)user.IsCsv)
        {
            process = processes[0];
        }
        else
        {
            process = processes[1];
        }

        ElectricCharger[] chargers = process.Read(stream);
        Selector selector = new Selector();
        Sorter sorter = new Sorter();
        ElectricCharger[] edited = null;
        try
        {
            switch (user.TypeOfAction)
            {
                case 1:
                    edited = selector.Select(chargers, 1, user.ValueForSelection.Split('|'));
                    break;
                case 2:
                    edited = selector.Select(chargers, 2, user.ValueForSelection.Split('|'));
                    break;
                case 3:
                    edited = selector.Select(chargers, 2, user.ValueForSelection.Split('|'));
                    break;
                case 4:
                    edited = sorter.Sort(chargers, true);
                    break;
                case 5:
                    edited = sorter.Sort(chargers, false);
                    break;
            }
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e);
            throw;
        }

        Stream[] output = { processes[0].Write(edited), processes[1].Write(edited) };
        return output;
    }

    public static async Task<Update[]> TryGetNewUpdates(Bot bot,
        CancellationToken cancellationToken)
    {
        try
        {
            Update[] updates = await bot.GetNewUpdatesAsync(cancellationToken);
            return updates;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return Array.Empty<Update>();
    }
    
    /// <summary>
    /// Makes all actions with user.
    /// </summary>
    /// <param name="bot">Bot object.</param>
    /// <param name="update">Information about received update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task MakeAction(Bot bot, Update update, CancellationToken cancellationToken)
    {
        long chatId = update.Message != null ? update.Message.Chat.Id : update.CallbackQuery != null ? update.CallbackQuery.From.Id : -1;
        UserInfo user = bot.Chats[chatId];
        if (chatId == -1) return;
        if (update.Message != null)
        {
            if (await Restart(bot, update, cancellationToken).ConfigureAwait(false)) return;
        }
        switch (bot.Chats[chatId].State)
        {
            case UserInfo.UserStates.None:
                try
                {
                    if (update.Message != null)
                    {
                        await bot.Updater.CommandHandlerAsync(bot.BotClient, update, bot.Chats[chatId], cancellationToken);    
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                break;
            case UserInfo.UserStates.WaitingForCallback:
                if (update.CallbackQuery != null)
                {
                    if (user.LastQueryId == update.CallbackQuery.Id) return;
                    user.LastQueryId = update.CallbackQuery.Id;
                    try
                    {
                        await bot.Updater.CallbackAnswerAsync(bot.BotClient, update.CallbackQuery, bot.Chats[chatId],
                            cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        user.State = UserInfo.UserStates.WaitingForCallback;
                    }    
                }

                if (user.TypeOfAction >= 4)
                {
                    try
                    {
                        Stream[] outputFiles = HandleFile(File.OpenRead(user.File), bot.Chats[chatId]);
                        await bot.Updater.SendFileAsync(bot.BotClient, chatId, outputFiles[0], cancellationToken,
                            $"{Path.GetFileNameWithoutExtension(user.File)}-edited.csv");
                        await bot.Updater.SendFileAsync(bot.BotClient, chatId, outputFiles[1], cancellationToken,
                            $"{Path.GetFileNameWithoutExtension(user.File)}-edited.json");
                        await bot.BotClient.SendStickerAsync(chatId,
                            new InputFileId("CAACAgIAAxkBAAELwhBl_AJMTBBTRo2lGYcGc5V3CrQdsQACxgIAAladvQqZEktEI61WKTQE"),
                            cancellationToken: cancellationToken);
                        user.State = UserInfo.UserStates.None;
                        user.File = null;
                        user.TypeOfAction = 0;
                    }
                    catch (CsvHelperException e)
                    {
                        Console.WriteLine(e);
                        await bot.BotClient.SendTextMessageAsync(chatId, "Какие-то проблемы в файле.",
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                break;
            case UserInfo.UserStates.UploadingFile:
                try
                {
                    if (update.Message != null && update.Message.Document != null)
                    {
                        await bot.Updater.UploadingFileHandlerAsync(bot.BotClient, update, bot.Chats[chatId],
                            cancellationToken);    
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                break;
            case UserInfo.UserStates.EnteringValueForSelection:
                if (update.Message is { Text: not null })
                {
                    user.ValueForSelection = update.Message.Text;
                    
                    try
                    {
                        Stream[] outputFiles = HandleFile(File.OpenRead(user.File), bot.Chats[chatId]);
                        await bot.Updater.SendFileAsync(bot.BotClient, chatId, outputFiles[0], cancellationToken,
                            $"{Path.GetFileNameWithoutExtension(user.File)}-edited.csv");
                        await bot.Updater.SendFileAsync(bot.BotClient, chatId, outputFiles[1], cancellationToken,
                            $"{Path.GetFileNameWithoutExtension(user.File)}-edited.json");
                        await bot.BotClient.SendStickerAsync(chatId,
                            new InputFileId("CAACAgIAAxkBAAELwhBl_AJMTBBTRo2lGYcGc5V3CrQdsQACxgIAAladvQqZEktEI61WKTQE"),
                            cancellationToken: cancellationToken);
                    }
                    catch (ArgumentException e)
                    {
                        await bot.BotClient.SendTextMessageAsync(chatId, $"{e.Message}",
                            cancellationToken: cancellationToken);
                    }
                    catch (CsvHelperException e)
                    {
                        Console.WriteLine(e);
                        await bot.BotClient.SendTextMessageAsync(chatId, "Какие-то проблемы в файле.",
                            cancellationToken: cancellationToken);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    user.State = UserInfo.UserStates.None;
                    user.File = null;
                    user.TypeOfAction = 0;
                }
                break;
        }
    }
}