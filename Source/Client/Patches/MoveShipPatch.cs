using HarmonyLib;
using Verse;
using Shared;
using static Shared.CommonEnumerators;
using SaveOurShip2;
using Shared.SOS2RTCompat;
using GameClient.Managers;
using GameClient.Values;
using GameClient.TCP;
using GameClient.Misc;
namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(ShipInteriorMod2), nameof(ShipInteriorMod2.MoveShip))]
    public static class LandShipCheckPost
    {
        [HarmonyPostfix]
        public static void DoPost(Building core, Map targetMap)
        {
            if (Network.State == ClientNetworkState.Connected)
            {
                ClientValues.ManageDevOptions();

                GameParameterManager.SetScenario(SessionValues.ScenarioFile);
                GameParameterManager.SetStoryteller(SessionValues.StorytellerFile);
                GameParameterManager.SetDifficulty(SessionValues.DifficultyFile);

                Map map = core.Map;
                if (!targetMap.IsSpace() && ShipInteriorMod2.FindPlayerShipMap() == null)
                {
                    Printer.Warning("[SOS2]Deleting empty space map", LogImportanceMode.Verbose);
                    PlayerSettlementData settlementData = new PlayerSettlementData();
                    settlementData._settlementFile = new SpaceSettlementFile(-1);
                    settlementData._settlementFile.Tile = Main.shipTile;
                    Main.shipTile = -1;
                    settlementData._stepMode = SettlementStepMode.Remove;

                    Network.Listener.EnqueueModdedPacket(ModdedPacketTypes.SpaceSettlement, settlementData);

                    SaveManager.ForceSave();
                }
            }
        }
    }
}
