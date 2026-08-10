using NetCord;
using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting;

public class MapVoteHelper(RestClient restClient, DiscordConfigHelper config)
{
    public async Task<MapThreadUpvotes> GetVotesFromThread(ulong threadId)
    {
        var threads = await restClient.GetActiveGuildThreadsAsync(config.GuildId);
        
        var forumThread = threads.First(i => i.Id == threadId);

        var downvoteReactions = await forumThread.GetMessageReactionsAsync(threadId, new ReactionEmojiProperties("👎")).ToArrayAsync();
        var upvoteReactions = await forumThread.GetMessageReactionsAsync(threadId, new ReactionEmojiProperties("👍")).ToArrayAsync();
        
        var reactionDictionary = new Dictionary<User, string>()
            .Concat(upvoteReactions.Select(i => new KeyValuePair<User, string>(i, "👍")))
            .Concat(downvoteReactions.Select(i => new KeyValuePair<User, string>(i, "👎")))
            .DistinctBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var submissionOwner = await GetOwnerFromThread(forumThread);
        
        downvoteReactions = reactionDictionary.Where(i => i.Value == "👎").Select(i => i.Key).ToArray();
        upvoteReactions = reactionDictionary.Where(i => i.Value == "👍").Select(i => i.Key).ToArray();

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