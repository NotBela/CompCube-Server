using System.Reflection.PortableExecutable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoungeSaber_Server.Models;

public class Map
{
    public Difficulties Difficulty { get; private set; }
    public Characteristics Characteristic { get; private set; }
    
    public string Hash { get; private set; }

    private Map(Difficulties difficulty, Characteristics characteristic, string hash)
    {
        Difficulty = difficulty;
        Hash = hash;
        Characteristic = characteristic;
    }

    //TODO: add map info parsing
    public static Map Parse(JObject json)
    {
        return new Map(Difficulties.ExpertPlus, Characteristics._Standard,"319503F83B147A7F864CC2301F4AE01AD754CCB6");
    }
    
    
    public enum Difficulties
    {
        Easy,
        Normal,
        Hard,
        Expert,
        ExpertPlus
    }

    public enum Characteristics
    {
        _Standard,
        _OneSaber,
        _NoArrows,
        _360Degree,
        _90Degree,
        _Lawless
    }
    
    public enum MapTypes
    {
        Acc,
        Speed,
        Tech,
        Midspeed,
        Balanced,
        Extreme
    }
}