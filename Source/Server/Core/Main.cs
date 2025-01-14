using GameServer.Misc;
using HarmonyLib;
using Shared;

namespace GameServer.SOS2RTCompat
{
    [RTStartup]
    public static class Main
    {
        static Main() 
        {
            Harmony harmony = new Harmony("SOSRTCompatServer");
            harmony.PatchAll();
            IDManager.SetCurrentIDOnLoad();
            Printer.Warning("[SOS2] Save our ship 2 patch loaded, welcome home captains.");
        }
    }
}
