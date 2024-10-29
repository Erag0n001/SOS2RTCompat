using HarmonyLib;
using System.Reflection;
using GameClient;
using SaveOurShip2;
using Shared;
namespace GameClient.SOS2RTCompat
{
    [RTStartup]
    public static class Main
    {
        private static readonly string patchID = "SOS2RTCompat";
        public static int shipTile;
        static Main()
        {
            Logger.Warning("[SOS2] patch loaded");
            LoadHarmonyPatches();
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
