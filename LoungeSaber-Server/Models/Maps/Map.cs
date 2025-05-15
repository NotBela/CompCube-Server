using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models.Maps;

public class Map
{
    public List<MapDifficulty> difficulties { get; set; }
    
    public string hash { get; private set; }

    public Map(MapDifficulty[] difficulties, string hash)
    {
        this.hash = hash;
        this.difficulties = difficulties.ToList();
    }
    
    public static Map Parse(JObject json) => json.ToObject<Map>()!;
}