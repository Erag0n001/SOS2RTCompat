using HarmonyLib;
using Verse;
using Shared;
using static Shared.CommonEnumerators;
using GameClient;
using SaveOurShip2;
using GameClient.Values;
using GameClient.Managers;
using GameClient.TCP;
namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(ShipInteriorMod2), nameof(ShipInteriorMod2.GeneratePlayerShipMap))]
    public static class GeneratePlayerShipMapPost
    {
        [HarmonyPostfix]
        public static void DoPost(Map __result)
        {
            if (Network.state == ClientNetworkState.Connected)
            {
                if (__result != null)
                {
                    ClientValues.ManageDevOptions();

                    GameParameterManager.SetScenario(SessionValues.scenarioFile);
                    GameParameterManager.SetStoryteller(SessionValues.storytellerFile);
                    GameParameterManager.SetDifficulty(SessionValues.difficultyFile);

                    Main.shipTile = __result.Tile;
                    SpaceSettlementManagerHelper.SendSettlementToServer(__result);


                    SaveManager.ForceSave();
                }
            }
        }
    }
}
