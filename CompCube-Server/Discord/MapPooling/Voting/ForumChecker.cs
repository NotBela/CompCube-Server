using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class ForumChecker
{
    private readonly RestClient _restClient;
    private readonly DiscordConfigHelper _configHelper;
    
    public ForumChecker(RestClient restClient, DiscordConfigHelper configHelper)
    {
        _restClient = restClient;
        _configHelper = configHelper;
        
        Task.Factory.StartNew(CheckMaps, TaskCreationOptions.LongRunning);
    }

    private async Task CheckMaps()
    {
        while (true)
        {
            var activeThreads = await _restClient.GetActiveGuildThreadsAsync(_configHelper.GuildId);

            var self = await _restClient.GetCurrentApplicationAsync();

            activeThreads = activeThreads.Where(i => i.OwnerId == self.Id).ToArray();

            foreach (var thread in activeThreads)
            {
                
            }
        }
    }
}