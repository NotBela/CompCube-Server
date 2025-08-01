using BeatSaverSharp;
using BeatSaverSharp.Models;

namespace LoungeSaber_Server.BeatSaverApi;

public static class BeatSaverApiWrapper
{
    private static readonly BeatSaverSharp.BeatSaver BeatSaver = new(new BeatSaverOptions("LoungeSaber-Server", new Version("1.0.0")));

    public static async Task<Beatmap?> GetBeatmapFromHash(string hash) => await BeatSaver.BeatmapByHash(hash);
}