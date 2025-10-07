using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace LoungeSaber_Server.Models.Events;

[method: Newtonsoft.Json.JsonConstructor]
public class EventData(string eventName, string displayName, string description)
{
    [JsonProperty(PropertyName = "eventName")]
    public string EventName => eventName;
    
    [JsonProperty(PropertyName = "displayName")]
    public string DisplayName => displayName;
    
    [JsonProperty(PropertyName = "description")]
    public string Description => description;
}