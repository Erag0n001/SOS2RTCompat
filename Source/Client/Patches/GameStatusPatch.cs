using GameClient;
using GameClient.TCP;
using HarmonyLib;
using Verse;
using static Shared.CommonEnumerators;
namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(Game), nameof(Game.LoadGame))]
    public static class LoadModePatch
    {
        [HarmonyPostfix]
        public static void GetIDFromExistingGame()
        {
            if (Network.state == ClientNetworkState.Connected)
            {
                Main.GetShipTile();
            }
        }
    }
}