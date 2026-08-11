using CompCube_Models.Models.Map;
using CompCube_Server.Data;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace CompCube_Server.Discord.MapPooling.Commands;

public class ForcedMapPoolControls(MapData mapData, MapQueue queue) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("forceadd", "Force adds a map to the pool", Contexts = [InteractionContextType.Guild], DefaultGuildPermissions = Permissions.Administrator)]
    public InteractionMessageProperties Add(string hash, string difficulty, string category)
    {
        if (!Enum.TryParse<VotingMap.Category>(category, out var categoryResult))
            return "Failed to parse category!";

        if (!Enum.TryParse<VotingMap.DifficultyType>(difficulty, out var difficultyResult))
            return "Failed to parse difficulty!";
        
        mapData.AddMap(new VotingMap(hash, difficultyResult, categoryResult));

        return $"Forcefully added {hash} to the map pool.";
    }

    [SlashCommand("forcequeue", "Force adds a map to the queue", Contexts = [InteractionContextType.Guild],
        DefaultGuildPermissions = Permissions.Administrator)]
    public InteractionMessageProperties Queue(string hash, string difficulty, string category)
    {
        if (!Enum.TryParse<VotingMap.Category>(category, out var categoryResult))
            return "Failed to parse category!";

        if (!Enum.TryParse<VotingMap.DifficultyType>(difficulty, out var difficultyResult))
            return "Failed to parse difficulty!";
        
        queue.AddToQueue(new VotingMap(hash, difficultyResult, categoryResult));
        
        return "Forcefully added " + hash + " to the queue.";
    }
    
    
}