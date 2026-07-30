# Contributing

Prerequisites:
- MySQL Server (8 or later)

Steps:
1. `git clone`
2. Create a file called appsettings.json in CompCube-Server/bin/Release/net10.0 and paste the following into it:
```json
{
  // Discord configuration. Leave UseDiscordIntegration disabled if you do not plan on contributing to the discord bot! 
  "Discord": {
    "UseDiscordIntegration": false,
    "Token": "",
    "MatchLoggingChannelId": -1,
    "EventsLoggingChannelId": -1
  },
  "Server": {
    "Season": 0, // Determines what instance of saved ranking data should be used
    "WebsocketListeningPort": 8008,
    "ApiListeningPort": 7198,
    "AllowedModVersions":[
      "0.1.1" // Controls what client mod versions are allowed to join
    ]
  },
  // Controls the data returned by the Contributors API Endpoint
  "Contributors": [
    {
      "name": "Bela",
      "role": "Developer",
      "profilePictureLink": "https://cdn.scoresaber.com/avatars/76561199003743737.jpg"
    },
    {
      "name": "Speecil",
      "role": "Developer",
      "profilePictureLink": "https://cdn.assets.beatleader.com/76561199077754911R24.png"
    }
  ],
  
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  // Connection string to connect to a database. Point this to your MySQL Server
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  
  "AllowedHosts": "*",
}
```
3. Build and run
