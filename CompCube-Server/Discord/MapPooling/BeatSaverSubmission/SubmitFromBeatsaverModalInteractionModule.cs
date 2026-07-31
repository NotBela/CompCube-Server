using CompCube_Server.Logging;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class SubmitFromBeatsaverModalInteractionModule(IConfiguration config, RestClient client) : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatsaverModal")]
    public async Task SubmitMapFromBeatsaverModalInteraction()
    {
        var category = Context.Components.OfType<Label>().Select(i => i.Component).OfType<StringMenu>().First().SelectedValues?.First() ?? throw new Exception("Could not get category!");
        var bsr = Context.Components.OfType<Label>().Select(i => i.Component).OfType<TextInput>().First().Value;
        var difficulty = Context.Components.OfType<Label>().Select(i => i.Component).OfType<StringMenu>().Last().SelectedValues?.First() ?? throw new Exception("Could not get difficulty!");

        var poolingChannelId = config.GetSection("Discord").GetSection("PoolingChannelIds").GetValue<ulong>(category);

        var forumThread = await client.CreateForumGuildThreadAsync(poolingChannelId, new ForumGuildThreadProperties("test", new ForumGuildThreadMessageProperties()
        {
            Content = bsr,
        }));
        
        Console.WriteLine(category);
        Console.WriteLine(bsr);
        Console.WriteLine(difficulty);

        // var poolingChannel = config.GetSection("Discord").GetSection("PoolingChannelIds").GetValue<ulong>();

        // await Context.Interaction.SendResponseAsync(InteractionCallback.Message("this will be deleted in 5 seconds lol"));
        //
        // await Task.Delay(5000);
        //
        // await Context.Interaction.DeleteResponseAsync();
    }
}