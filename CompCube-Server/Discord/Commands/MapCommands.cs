using System.IO.Compression;
using CompCube_Models.Models.Map;
using CompCube_Server.Api.BeatSaver;
using CompCube_Server.Api.Controllers;
using CompCube_Server.SQL;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace CompCube_Server.Discord.Commands;

public class MapCommands(BeatSaverApiWrapper beatSaverApi, MapData mapData) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("addmap", "add a map")]
    public async Task<InteractionMessageProperties> AddMap(string key, string diff, string category, string categoryLabel)
    {
        var beatmap = await beatSaverApi.GetBeatmapFromKey(key);

        if (beatmap == null) 
            return "Invalid key!";

        if (!Enum.TryParse<VotingMap.Category>(category, out var mapCategory))
            return "Could not parse category!";

        if (!Enum.TryParse<VotingMap.DifficultyType>(diff, out var difficulty))
            return "Could not parse difficulty!";

        var pathToBeatmap = Path.Combine(MapApiController.BeatmapsPath, beatmap.LatestVersion.Hash + ".zip");

        if (!File.Exists(pathToBeatmap))
        {
            var beatmapBytes = await beatmap.LatestVersion.DownloadZIP();

            if (beatmapBytes == null)
            {
                return "Could not download beatmap!";
            }
            
            await File.WriteAllBytesAsync(pathToBeatmap, beatmapBytes);
        }
        
        mapData.AddMap(new VotingMap(beatmap.LatestVersion.Hash, difficulty, mapCategory, categoryLabel));

        return $"{beatmap.Name} added to pool.";
    }
}