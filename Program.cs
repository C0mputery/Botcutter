using DSharpPlus;
using DSharpPlus.SlashCommands;

namespace CUTSBot
{
    internal class Program
    {
        static async Task Main(string[] args)
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
            await Task.Delay(-1);
        }
    }
}