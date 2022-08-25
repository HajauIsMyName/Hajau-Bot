using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Database dataObject = new Database();
            new Program().RunBotAsync().GetAwaiter().GetResult();
        }

        private DiscordSocketClient _client;

        private CommandService _commands;

        private IServiceProvider _services;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            Env.Load();
            Env.TraversePath().Load();

            _client.Log += _client_Log;

            await RegisterCommandAsync();

            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("TOKEN"));

            await _client.StartAsync();

            _client.Ready += BotReady;

            await Task.Delay(-1);
        }

        public async Task RegisterCommandAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private Task BotReady()
        {
            Console.WriteLine($"{_client.CurrentUser} is online");
            return Task.CompletedTask;
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            var context = new SocketCommandContext(_client, message);

            if (message.Author.IsBot)
            {
                return;
            }

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _services);
                if (!result.IsSuccess)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}