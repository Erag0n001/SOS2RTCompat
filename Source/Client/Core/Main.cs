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
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (type.GetCustomAttributes(typeof(RTManager), false).Length != 0)
                {
                    try { Master.managerDictionary[type.Name] = type.GetMethod("ParsePacket"); }
                    catch (Exception exception) { Printer.Error($"{type.Name} failed to load\n{exception}"); }
                }
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
