using CompCube_Server.Logging;
using Discord;
using Discord.WebSocket;

namespace CompCube_Server.Discord;

public class DiscordBotManager
{
    private readonly Logger _logger;
    
    private readonly DiscordSocketClient _client;
    
    private readonly string _token;

    public DiscordBotManager(IConfiguration config, Logger logger)
    {
        _logger = logger;
        
        _client = new DiscordSocketClient();
        
        _token = config.GetSection("Discord").GetValue<string>("Token") ?? throw new Exception("Token is missing!");
        
        Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);

        _client.Log += HandleLog;
    }

    private async Task Run()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        
        await _client.StartAsync();
        await Task.Delay(-1);
    }

    private Task HandleLog(LogMessage log)
    {
        _logger.Info(log.ToString());
        return Task.CompletedTask;
    }
}