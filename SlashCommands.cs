using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CUTSBot
{
    public class SlashCommands : ApplicationCommandModule
    {
        [SlashCommand("List-Tabg-Servers", "Prints out all the active Tabg community servers")]
        public async Task ListTabgServers(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
            try
            {
                string serverListJson = await GetServerList();
                ServerList? serverList = JsonConvert.DeserializeObject<ServerList>(serverListJson);
                string serverInfoString = $"# TABG Community Server List{Environment.NewLine}";
                List<Server> sortedServers = serverList.Servers.OrderByDescending(server => server.PlayersOnServer).ToList();
                foreach (Server server in sortedServers)
                {
                    string cleanServerName = Regex.Replace(server.ServerName, "<.*?>", string.Empty).Trim();
                    cleanServerName = Regex.Replace(cleanServerName, @"\s+", " ");
                    string serverStatus = server.AcceptingPlayers ? "Waiting for players" : "Game in progress";
                    serverInfoString += $"**Name:** {cleanServerName} | **Match State:** {serverStatus} | **Players In Match:** {server.PlayersOnServer}/{server.MaxPlayers}";
                    if (server.Passworded)
                    {
                        serverInfoString += " | **Is Password Protected**";
                    }
                    serverInfoString += Environment.NewLine;
                }
                if (serverInfoString.Length > 2000)
                {
                    serverInfoString = serverInfoString.Substring(0, 1999);
                }
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent(serverInfoString));
            } catch (Exception)
            {
                await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("An Error, Fuck"));
            }
        }

        public class Server
        {
            public string? Id { get; set; }
            public string? ServerName { get; set; }
            public int PlayersOnServer { get; set; }
            public int SpotsReserved { get; set; }
            public int MaxPlayers { get; set; }
            public bool AcceptingPlayers { get; set; }
            public string? ServerDescription { get; set; }
            public string? SquadMode { get; set; }
            public string? Gamemode { get; set; }
            public bool Passworded { get; set; }
        }

        public class ServerList
        {
            public List<Server>? Servers { get; set; }
        }

        public static async Task<string> GetServerList()
        {
            var serverInfo = new
            {
                Version = "693fa81cb14a5bc46bf18456d161a7c6"
            };
            return await HTTPJsonPostCall("https://tabgcommunitybackend.azurewebsites.net/GetServerList", System.Text.Json.JsonSerializer.Serialize(serverInfo));
        }
        private static async Task<string> HTTPJsonPostCall(string fullUrl, string jsonBody)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await httpClient.PostAsync(fullUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        throw new HttpRequestException($"HTTP error: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception: {ex.Message}");
                }
            }
        }
    }
}
