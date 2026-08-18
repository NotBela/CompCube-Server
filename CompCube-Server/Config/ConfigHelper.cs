namespace CompCube_Server.Config;

public class ConfigHelper(IConfiguration config)
{
    public int Season => config.GetSection("Gameplay").GetValue("Season", 0);

    public string Secret => config.GetSection("Api").GetValue<string>("Secret")!;
}