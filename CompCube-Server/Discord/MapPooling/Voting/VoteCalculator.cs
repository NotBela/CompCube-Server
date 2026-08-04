using NetCord;
using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class VoteCalculator(RestClient restClient, DiscordConfigHelper config)
{
    public async Task<MapThreadUpvotes> GetUpvotesFromThread(ulong threadId)
    {
        var threads = await restClient.GetActiveGuildThreadsAsync(config.GuildId);
        
        var forumThread = threads.First(i => i.Id == threadId);

        var downvoteReactions = await forumThread.GetMessageReactionsAsync(threadId, new ReactionEmojiProperties("👎")).ToArrayAsync();
        var upvoteReactions = await forumThread.GetMessageReactionsAsync(threadId, new ReactionEmojiProperties("👍")).ToArrayAsync();

        var submissionOwner = await GetOwnerFromThread(forumThread);
        
        downvoteReactions = downvoteReactions.Where(i => i.Id != submissionOwner.Id).ToArray();
        upvoteReactions = upvoteReactions.Where(i => i.Id != submissionOwner.Id).ToArray();

        return new MapThreadUpvotes(upvoteReactions, downvoteReactions, submissionOwner);
    }

    public async Task<User> GetOwnerFromThread(GuildThread thread)
    {
        var message = await thread.GetMessageAsync(thread.Id);

        var submittedByFieldData = message.Embeds[0].Fields.First(i => i.Name == "Submitted by:").Value;

        var userIdString = string.Join("", submittedByFieldData.ToCharArray().Where(char.IsDigit).ToArray());
            
        return await restClient.GetUserAsync(ulong.Parse(userIdString));
    }
}

public class MapThreadUpvotes(User[] upvotes, User[] downvotes, User submissionOwner)
{
    public IReadOnlyList<User> Upvotes => upvotes.AsReadOnly();
    public IReadOnlyList<User> Downvotes => downvotes.AsReadOnly();
    
    public readonly User SubmissionOwner = submissionOwner;
}