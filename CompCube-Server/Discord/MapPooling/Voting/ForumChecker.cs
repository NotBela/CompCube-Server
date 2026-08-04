using NetCord;
using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class ForumChecker
{
    private readonly RestClient _restClient;
    private readonly DiscordConfigHelper _configHelper;
    private readonly MapVoteHelper _mapVoteHelper;
    
    public ForumChecker(RestClient restClient, DiscordConfigHelper configHelper, MapVoteHelper mapVoteHelper)
    {
        _restClient = restClient;
        _configHelper = configHelper;
        _mapVoteHelper = mapVoteHelper;
        
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
                await CheckForum((ForumGuildThread) thread);
            
            await Task.Delay(new TimeSpan(0, 1, 0));
        }
    }

    private async Task CheckForum(ForumGuildThread thread)
    {
        if (thread.CreatedAt.Date.AddMinutes(7) > DateTimeOffset.Now.Date)
            return;

        if (thread.CreatedAt.Date.AddMinutes(14) <= DateTimeOffset.Now.Date)
        {
            await _restClient.SendMessageAsync(thread.Id, new MessageProperties()
            {
                Embeds = [
                    new EmbedProperties()
                    {
                        Description = "This map has been denied due to a verdict not being made after 14 days."
                    }
                ]
            });

            await thread.ModifyAsync(options => options.WithArchived().WithLocked());
            return;
        }
                
        var voteState = await _mapVoteHelper.GetUpvotesFromThread(thread.Id);

        if (voteState.Downvotes.Count == 1)
        {
            await DenyMap(thread);
            return;
        }

        if (voteState.Upvotes.Count == 1)
            await AcceptMap(thread);
    }

    private async Task AcceptMap(ForumGuildThread forumThread)
    {
        var channel = await _restClient.GetChannelAsync(forumThread.OwnerId);

        var forumChannel = (ForumGuildChannel)channel;

        var acceptedTag = forumChannel.AvailableTags.FirstOrDefault(i => i.Name == "Accepted");
        
        if (acceptedTag == null)
            throw new Exception("Accepted tag not found");
        
        await forumThread.ModifyAsync(options => options.WithArchived().WithLocked().WithAppliedTags([acceptedTag.Id]));
    }

    private async Task DenyMap(ForumGuildThread forumThread)
    {
        var channel = await _restClient.GetChannelAsync(forumThread.OwnerId);

        var forumChannel = (ForumGuildChannel)channel;

        var acceptedTag = forumChannel.AvailableTags.FirstOrDefault(i => i.Name == "Denied");
        
        if (acceptedTag == null)
            throw new Exception("Denied tag not found");
        
        await forumThread.ModifyAsync(options => options.WithArchived().WithLocked().WithAppliedTags([acceptedTag.Id]));
    }
}