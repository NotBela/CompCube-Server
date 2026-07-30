using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class SubmitFromBeatsaverModalInteractionModule : ComponentInteractionModule<ModalInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatsaverModal")]
    public async Task SubmitMapFromBeatsaverModalInteraction()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("this will be deleted in 5 seconds lol"));
        
        await Task.Delay(5000);

        await Context.Interaction.DeleteResponseAsync();
    }
}