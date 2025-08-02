using LoungeSaber_Server.Installer;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;

namespace LoungeSaber_Server
{
    public class Program
    {
        public const bool Debug = true;
        
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder();

            BindingsInstaller.InstallBindings(builder.Services);
            
            builder.Services.AddDiscordGateway().AddApplicationCommands();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var host = builder.Build();
            
            if (host.Environment.IsDevelopment())
            {
                host.UseSwagger();
                host.UseSwaggerUI();
            }

            host.UseHttpsRedirection();
            host.MapControllers();
            
            host.AddModules(typeof(Program).Assembly);

            host.UseGatewayHandlers();

            await host.RunAsync();
        }
    }
}