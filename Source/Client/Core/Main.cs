using HarmonyLib;
using System.Reflection;
using GameClient;
using SaveOurShip2;
using Shared;
using System.IO;
using System;
using Verse;
using GameClient.Misc;
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
            LoadHarmonyPatches();

        }

        public static void LoadClientAssembly() 
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;

            static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
            {
                var assemblyPath = Path.Combine("..", "Assemblies", $"ClientDLL.dll");

                return File.Exists(assemblyPath) ? Assembly.LoadFrom(assemblyPath) : null;
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
