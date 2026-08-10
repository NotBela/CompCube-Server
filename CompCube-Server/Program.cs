using CompCube_Server.Api.BeatSaver;
using CompCube_Server.Api.Controllers;
using CompCube_Server.Data;
using CompCube_Server.Discord;
using CompCube_Server.Discord.MapPooling;
using CompCube_Server.Discord.MapPooling.Voting;
using CompCube_Server.Gameplay.Match;
using CompCube_Server.Gameplay.Matchmaking;
using CompCube_Server.Interfaces;
using CompCube_Server.Logging;
using CompCube_Server.Networking.ServerStatus;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server;

public class Program
{
    private static bool _useDiscordIntegration = false;
    
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            // ContentRootPath = AppDomain.CurrentDomain.BaseDirectory,
        });

        _useDiscordIntegration = builder.Configuration.GetSection("Discord").GetValue<bool>("UseDiscordIntegration");
        
        InstallBindings(builder.Services);
        
        if (_useDiscordIntegration)
            builder.Services.AddDiscordGateway(options => options.Intents = GatewayIntents.All).AddApplicationCommands().AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>().AddComponentInteractions<ModalInteraction, ModalInteractionContext>().AddGatewayHandlers(typeof(Program).Assembly);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Beatmaps"));

        var webSocketPort = builder.Configuration.GetSection("Server").GetValue("WebsocketListeningPort", -1);

        if (webSocketPort == -1)
        {
            Console.WriteLine("No websocket port configured. Defaulting to 8008");
            webSocketPort = 8008;
        }
        
        var apiPort = builder.Configuration.GetSection("Server").GetValue("ApiListeningPort", -1);

        if (apiPort == -1)
        {
            Console.WriteLine("No API port configured. Defaulting to 7198");
            apiPort = 7198;
        }

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(webSocketPort);
            options.ListenAnyIP(apiPort);
        });
        
        var host = builder.Build();
            
        host.UseSwagger();
        host.UseSwaggerUI();

        // host.UseHttpsRedirection();
        host.MapControllers();

        if (_useDiscordIntegration)
        {
            host.AddModules(typeof(Program).Assembly);
        }
        
        var connectionManager = host.Services.GetRequiredService<ConnectionManager>();
        if (_useDiscordIntegration)
            host.Services.GetRequiredService<ForumChecker>();

        host.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30),
        });

        host.Map("", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource();
                
                await connectionManager.HandleWebSocket(webSocket, socketFinishedTcs);

                await socketFinishedTcs.Task;
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        });
        
        host.Run();
    }
        
    private static void InstallBindings(IServiceCollection services)
    {
        services.AddSingleton<Logger>();
        
        services.AddTransient<MatchLog>();
        services.AddTransient<MapData>();
        services.AddTransient<UserData>();
        services.AddTransient<RankingData>();
        services.AddTransient<RankFetcher>();

        services.AddTransient<DbSession>();

        services.AddSingleton<ServerStatusManager>();
        
        services.AddSingleton<ConnectionManager>();

        services.AddSingleton<GameMatchFactory>();
        
        services.AddSingleton<QueueManager>();
        
        services.AddSingleton<IQueue, DebugQueue>();
        services.AddSingleton<IQueue, StandardCasualQueue>();
        services.AddSingleton<IQueue, StandardCompetitiveQueue>();

        services.AddSingleton<BeatSaverApiWrapper>();

        services.AddSingleton<LeaderboardApiController>();
        services.AddSingleton<MapApiController>();
        services.AddSingleton<ServerStatusApiController>();
        services.AddSingleton<UserApiController>();

        if (!_useDiscordIntegration) 
            return;
        
        services.AddSingleton<DiscordConfigHelper>();
        services.AddSingleton<MapVoteHelper>();
        services.AddSingleton<ForumChecker>();
    }
}