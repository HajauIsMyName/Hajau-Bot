using System.Data.SQLite;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Bot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private DiscordSocketClient _client;

        private Database dataObject = new Database();

        private string query;

        [Command("avatar")]
        [Alias("avatar", "av")]

        public async Task Avatar(IGuildUser user = null)
        {
            if (user == null)
            {
                user = (IGuildUser?)Context.Message.Author;
            }

            var embed = new EmbedBuilder();

            embed
                .WithAuthor(user)
                .WithTitle("Avatar")
                .WithImageUrl(user.GetAvatarUrl(size: 2048));

            await ReplyAsync(embed: embed.Build());
        }

        [Command("ping")]

        public async Task Ping()
        {
            await ReplyAsync($"🏓 Pong! ``{(Context.Client as DiscordSocketClient).Latency}ms``");
        }

        [Command("balance")]
        [Alias("balance", "bal")]

        public async Task Balance()
        {
            query = $"SELECT * FROM main WHERE userID = {Context.Message.Author.Id}";
            SQLiteCommand command = new SQLiteCommand(query, dataObject.DBConnection);
            dataObject.OpenConnection();
            var result = command.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();
                var embed = new EmbedBuilder();

                embed
                    .WithAuthor(name: $"{Context.Message.Author.Username}'s balance", iconUrl: Context.Message.Author.GetAvatarUrl())
                    .AddField("Wallet:", $":coin: {result["wallet"]}")
                    .AddField("Bank:", $":coin: {result["bank"]}");

                await ReplyAsync(embed: embed.Build());
            }

            else
            {
                query = "INSERT INTO main (`userID`, `wallet`, `bank`) VALUES (@userID, @wallet, @bank)";
                SQLiteCommand NewCommand = new SQLiteCommand(query, dataObject.DBConnection);
                NewCommand.Parameters.AddWithValue("@userID", Context.Message.Author.Id);
                NewCommand.Parameters.AddWithValue("@wallet", 0);
                NewCommand.Parameters.AddWithValue("@bank", 0);

                var NewResult = NewCommand.ExecuteNonQuery();

                var embed = new EmbedBuilder();

                embed
                    .WithAuthor(name: $"{Context.Message.Author.Username}'s balance", iconUrl: Context.Message.Author.GetAvatarUrl())
                    .AddField("Wallet:", 0)
                    .AddField("Bank:", 0);

                await ReplyAsync(embed: embed.Build());
            }

            dataObject.CloseConnection();
        }

        [Command("work")]
        [Alias("work", "job")]

        public async Task Work()
        {
            Random random = new Random();
            var earnings = random.Next(201);

            query = $"SELECT * FROM main WHERE userID = {Context.Message.Author.Id}";
            SQLiteCommand command = new SQLiteCommand(query, dataObject.DBConnection);
            dataObject.OpenConnection();
            var result = command.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();
                query = $"UPDATE main SET wallet = {(Int64)result["wallet"] + earnings} WHERE userID = {Context.Message.Author.Id}";
                command = new SQLiteCommand(query, dataObject.DBConnection);
                var NewResult = command.ExecuteNonQuery();

                var embed = new EmbedBuilder();

                embed
                    .WithAuthor(Context.Message.Author)
                    .WithDescription($"The boss paid you :coin: **{earnings}**");

                await ReplyAsync(embed: embed.Build());
            }

            else
            {
                query = "INSERT INTO main (`userID`, `wallet`, `bank`) VALUES (@userID, @wallet, @bank)";
                SQLiteCommand NewCommand = new SQLiteCommand(query, dataObject.DBConnection);
                NewCommand.Parameters.AddWithValue("@userID", Context.Message.Author.Id);
                NewCommand.Parameters.AddWithValue("@wallet", earnings);
                NewCommand.Parameters.AddWithValue("@bank", 0);

                var NewResult = NewCommand.ExecuteNonQuery();

                var embed = new EmbedBuilder();

                embed
                    .WithAuthor(Context.Message.Author)
                    .WithDescription($"The boss paid you :coin: **{earnings}**");

                await ReplyAsync(embed: embed.Build());
            }

            dataObject.CloseConnection();
        }
    }
}