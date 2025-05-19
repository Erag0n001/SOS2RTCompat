using System.Reflection;
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
            
            MethodInfo method = AccessTools.Method(typeof(MethodGatherer), "GetPacketHandlerAttributes");
            MethodInfo[] serverMethods = (MethodInfo[])method.Invoke(null, Assembly.GetExecutingAssembly().GetTypes().ToArray());
            for (int i = 0; i < serverMethods.Length; i++)
            {
                PacketHeader header = (PacketHeader)serverMethods[i].GetCustomAttribute<HandlesModdedPacket>().header;
                MethodGatherer.ServerMethodDictionary.Add(header,serverMethods[i]);
            }
        }
    }
}
