using BotLib;
using ElectricChargesLib;
using FileOpsLib;
using Telegram.Bot;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace HCW33;

public class Program
{
    static async Task Main(string[] args)
    {
        Bot bot = new Bot();
        Thread.Sleep(200);
        do
        {
            Update[] updates = await Catcher.TryGetNewUpdates(bot, bot.Token);
            if (updates != Array.Empty<Update>())
            {
                foreach (var up in updates)
                {
                    await Catcher.MakeAction(bot, up, bot.Token);
                }    
            }
            Thread.Sleep(400);
        } while (true);
    }
}