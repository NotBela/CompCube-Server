using CompCube_Models.Models.Map;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ComponentInteractions;

namespace CompCube_Server.Discord.MapPooling;

public class MapPoolButtonInteractionModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("submitMapFromBeatSaverButton")]
    public async Task SubmitMapFromBeatSaverInteraction()
    {
        var modal = new ModalProperties("submitMapFromBeatsaverModal", "Submit From BeatSaver")
        {
            Components = [new TextInputProperties("categoryMenu", TextInputStyle.Short,"test")]
        };
        
        await Context.Interaction.SendResponseAsync(InteractionCallback.Modal(modal));
    }
}