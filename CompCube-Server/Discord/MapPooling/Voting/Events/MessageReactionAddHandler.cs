using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace CompCube_Server.Discord.MapPooling.Voting.Events;

public class MessageReactionAddHandler(RestClient client, DiscordConfigHelper config, MapVoteHelper mapVoteHelper) : IMessageReactionAddGatewayHandler
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        var threads = await client.GetActiveGuildThreadsAsync(config.GuildId);
        
        var forumThread = threads.First(i => i.Id == args.ChannelId);

        var owner = await mapVoteHelper.GetOwnerFromThread(forumThread);

        if (owner.Id != args.UserId)
            return;

        await client.DeleteUserMessageReactionAsync(args.ChannelId, args.ChannelId, new ReactionEmojiProperties("👍"), args.UserId);
    }
}