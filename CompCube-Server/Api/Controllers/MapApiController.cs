using CompCube_Models.Models.Map;
using CompCube_Server.Api.BeatSaver;
using CompCube_Server.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace CompCube_Server.Api.Controllers;

[ApiController]
public class MapApiController : ControllerBase
{
    private readonly MapData _mapData;
    private readonly BeatSaverApiWrapper _beatSaver;
    
    public static readonly string BeatmapsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Beatmaps");

    public MapApiController(MapData mapData, BeatSaverApiWrapper beatSaver)
    {
        _mapData = mapData;
        _beatSaver = beatSaver;

        Directory.CreateDirectory(BeatmapsPath);
    }

    [HttpGet("/api/maps/hashes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string[]> GetAllMapHashes() => _mapData.GetAllMaps().Select(i => i.Hash).ToArray();

    [HttpGet("/api/maps/download/{hash}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DownloadMap(string hash)
    {
        var filePath = Path.Combine(BeatmapsPath, $"{hash}.zip");

        if (!System.IO.File.Exists(filePath))
            return NotFound();
        
        var bytes = System.IO.File.ReadAllBytes(filePath);
        
        return File(bytes, "application/zip", $"{hash}.zip");
    }

    [HttpGet("/api/maps/playlist")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<string> GetPlaylist()
    {
        var allMaps = _mapData.GetAllMaps();

        var songs = new List<PlaylistSong>();

        foreach (var song in allMaps)
        {
            if (songs.Any(i => i.Hash == song.Hash))
                continue;
            
            var allSimilarHashes = allMaps.Where(i => i.Hash.Equals(song.Hash, StringComparison.CurrentCultureIgnoreCase));

            var playlistSong = new PlaylistSong(song.Hash, allSimilarHashes.Select(i => i.Difficulty).ToArray());
            
            songs.Add(playlistSong);
        }

        var jObject = new JObject
        {
            {"playlistTitle", "CompCube Maps"},
            {"playlistAuthor", "CompCube Team"},
            {"songs", new JArray(songs.Select(i => i.GetJsonObject()))}
        };

        return jObject.ToString();
    }
}

public class PlaylistSong(string hash, VotingMap.DifficultyType[] difficultyTypes)
{
    public readonly string Hash = hash;

    public readonly VotingMap.DifficultyType[] DifficultyTypes = difficultyTypes;

    public JObject GetJsonObject()
    {
        var difficultiesObject = new JArray();

        foreach (var diff in DifficultyTypes)
        {
            difficultiesObject.Add(new JObject
            {
                {"characteristic", "Standard"},
                {"name", diff.ToString()}
            });
        }
        
        var jObject = new JObject
        {
            {"hash", Hash},
            {"difficulties", difficultiesObject}
        };

        return jObject;
    }
}