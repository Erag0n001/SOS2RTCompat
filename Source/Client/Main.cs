using System.Reflection;
using GameClient;
using Shared;

namespace RTPatch
{
    // This class is the entry point for your CLIENT patch.
    // It will start executing the moment the mod loads in the game.

    [RTStartup]
    public static class Main 
    {
        static Main()
        {
            // Put whatever you want to execute at boot in the CLIENT in here

            Logger.Warning($"Your custom assembly {Assembly.GetExecutingAssembly().GetName().Name} was loaded!");
        }
    }
}
