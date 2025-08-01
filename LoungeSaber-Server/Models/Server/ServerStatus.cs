using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Server;

public class ServerStatus(
    string[] allowedGameVersions,
    string[] allowedModVersions,
    ServerStatusManager.ServerState state)
{
    [JsonProperty("allowedGameVersions")]
    public readonly string[] AllowedGameVersions = allowedGameVersions;
    
    [JsonProperty("allowedModVersions")]
    public readonly string[] AllowedModVersions = allowedModVersions;
    
    [JsonProperty("state")]
    public readonly ServerStatusManager.ServerState State = state;
}