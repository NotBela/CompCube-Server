using System.Reflection;
using CompCube_Server.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CompCube_Server.Gameplay.Matchmaking;

public abstract class StandardQueue : IQueue
{
    public abstract string QueueName { get; }

    public abstract void AddClientToPool(IConnectedClient client);
}