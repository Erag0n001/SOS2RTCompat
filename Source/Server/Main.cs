using System.Reflection;
using GameServer;
using Shared;

namespace RTPatch
{
    // This class is the entry point for your SERVER patch.
    // It will start executing the moment the mod loads in the game.

    [RTStartup]
    public static class Main 
    {
        static Main()
        {
            // Put whatever you want to execute at boot in the SERVER in here

            Logger.Warning($"Your custom assembly {Assembly.GetExecutingAssembly().GetName().Name} was loaded!");
        }
    }
}
