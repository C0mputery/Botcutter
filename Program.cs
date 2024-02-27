using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace CUTSBot
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            bool connected = false;
            while (!connected)
            {
                try
                {
                    string Token = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Token.txt")).Trim();
                    DiscordClient discord = new DiscordClient(new DiscordConfiguration()
                    {
                        Token = Token,
                        TokenType = TokenType.Bot,
                        Intents = DiscordIntents.AllUnprivileged
                    });

                    SlashCommandsExtension slash = discord.UseSlashCommands();
                    slash.RegisterCommands<SlashCommands>();

                    await discord.ConnectAsync();
                    connected = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect: {ex.Message}");
                    await Task.Delay(5000); // Wait for 5 seconds before retrying
                }
            }
            await Task.Delay(-1);
        }
    }
}