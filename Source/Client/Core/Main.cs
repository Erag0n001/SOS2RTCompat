using HarmonyLib;
using System.Reflection;
using GameClient;
using SaveOurShip2;
using Shared;
using System.IO;
using System;
using Verse;
using GameClient.Misc;
using GameClient.Core;
using System.Collections.Generic;
using System.Linq;
namespace GameClient.SOS2RTCompat
{
    [StaticConstructorOnStartup]
    public static class Main
    {
        private static readonly string patchID = "SOS2RTCompat";
        public static int shipTile;
        static Main()
        {
            Printer.Warning("[SOS2] patch loaded");
            LoadAllManagers();
            LoadHarmonyPatches();
        }

        public static void LoadAllManagers() 
        {
            MethodInfo method = AccessTools.Method(typeof(MethodGatherer), "GetPacketHandlerAttributes");
            MethodInfo[] clientMethods = (MethodInfo[])method.Invoke(null, Assembly.GetExecutingAssembly().GetTypes().ToArray());
            for (int i = 0; i < clientMethods.Length; i++)
            {
                PacketHeader header = (PacketHeader)clientMethods[i].GetCustomAttribute<HandlesModdedPacket>().header;
                MethodGatherer.ClientMethodDictionary.Add(header, clientMethods[i]);
            }
        }

        public static void GetShipTile()
        {
            if (ShipInteriorMod2.FindPlayerShipMap() == null)
            {
                shipTile = -1;
            }
            else
            {
                shipTile = ShipInteriorMod2.FindPlayerShipMap().Tile;
            }
        }
        public static void LoadHarmonyPatches()
        {
            Harmony harmony = new Harmony(patchID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

}
