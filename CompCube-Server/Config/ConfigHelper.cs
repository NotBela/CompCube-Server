namespace CompCube_Server.Config;

public class ConfigHelper(IConfiguration config)
{
    public int Season => config.GetSection("Gameplay").GetValue("Season", 0);

    public string Secret => config.GetSection("Api").GetValue<string>("Secret")!;
    
    public bool WhitelistEnabled => config.GetSection("Whitelist").GetValue("Enabled", false);
    
    public string[] WhitelistedIds => config.GetSection("Whitelist").GetSection("AllowedIds").Get<string[]>() ?? [];
}