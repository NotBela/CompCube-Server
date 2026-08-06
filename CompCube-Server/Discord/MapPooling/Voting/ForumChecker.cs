using CompCube_Server.Logging;
using NetCord;
using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class ForumChecker
{
    private readonly RestClient _restClient;
    private readonly DiscordConfigHelper _configHelper;
    private readonly MapVoteHelper _mapVoteHelper;
    private readonly Logger _logger;
    
    public ForumChecker(RestClient restClient, DiscordConfigHelper configHelper, MapVoteHelper mapVoteHelper, Logger logger)
    {
        _restClient = restClient;
        _configHelper = configHelper;
        _mapVoteHelper = mapVoteHelper;
        _logger = logger;
        
        Console.WriteLine("started");
        
        Task.Factory.StartNew(CheckMaps, TaskCreationOptions.LongRunning);
    }

    private async Task CheckMaps()
    {
        while (true)
        {
            try
            {
                Console.WriteLine("checking maps");

                var activeThreads = await _restClient.GetActiveGuildThreadsAsync(_configHelper.GuildId);

                Console.WriteLine(activeThreads.Count);

                var self = await _restClient.GetCurrentApplicationAsync();

                activeThreads = activeThreads.Where(i => i.OwnerId == self.Id).ToArray();

                Console.WriteLine(activeThreads.Count);

                foreach (var thread in activeThreads)
                    await CheckForum(thread);

                await Task.Delay(new TimeSpan(0, 1, 0));
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }
    }

    private async Task CheckForum(GuildThread thread)
    {
        Console.WriteLine($"checking for {thread.Id}");
        
        if (thread.CreatedAt.AddMinutes(7) > DateTimeOffset.Now)
            return;

        if (thread.CreatedAt.AddMinutes(14) <= DateTimeOffset.Now)
        {
            Console.WriteLine("rejected for inactivity");
            
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
        
        var voteState = await _mapVoteHelper.GetVotesFromThread(thread.Id);

        if (voteState.Downvotes.Count >= 1)
        {
            await DenyMap(thread, voteState);
            return;
        }

        if (voteState.Upvotes.Count >= 1)
            await AcceptMap(thread, voteState);
    }

    private async Task AcceptMap(GuildThread forumThread, MapThreadUpvotes voteState)
    {
        Console.WriteLine("accepted");
        
        await forumThread.SendMessageAsync(new MessageProperties()
        {
            Embeds = [
                new EmbedProperties()
                {
                    Description = $"✅ This map has been accepted with {voteState.Upvotes.Count} upvotes and {voteState.Downvotes.Count} downvotes."
                }
            ]
        });

        await forumThread.ModifyAsync(options =>
            options.WithName($"[✅] {forumThread.Name}").WithArchived().WithLocked()); // .WithAppliedTags([acceptedTag.Id]));
    }

    private async Task DenyMap(GuildThread forumThread, MapThreadUpvotes voteState)
    {
        Console.WriteLine("denied");

        await forumThread.SendMessageAsync(new MessageProperties()
        {
            Embeds = [
                new EmbedProperties()
                {
                    Description = $"❌ This map has been denied with {voteState.Upvotes.Count} upvotes and {voteState.Downvotes.Count} downvotes."
                }
            ]
        });
        
        await forumThread.ModifyAsync(options => options.WithName($"[❌] {forumThread.Name}").WithArchived().WithLocked()); // .WithLocked().WithAppliedTags([acceptedTag.Id]));
    }
}