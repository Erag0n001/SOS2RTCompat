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
            if (Network.state == ClientNetworkState.Connected)
            {
                ClientValues.ManageDevOptions();

                GameParameterManager.SetScenario(SessionValues.scenarioFile);
                GameParameterManager.SetStoryteller(SessionValues.storytellerFile);
                GameParameterManager.SetDifficulty(SessionValues.difficultyFile);

                Map map = core.Map;
                if (!targetMap.IsSpace() && ShipInteriorMod2.FindPlayerShipMap() == null)
                {
                    Printer.Warning("[SOS2]Deleting empty space map", LogImportanceMode.Verbose);
                    PlayerSettlementData settlementData = new PlayerSettlementData();
                    settlementData._settlementFile = new SpaceSettlementFile(-1);
                    settlementData._settlementFile.Tile = Main.shipTile;
                    Main.shipTile = -1;
                    settlementData._stepMode = SettlementStepMode.Remove;

                    Packet packet = Packet.CreatePacketFromObject(nameof(SpaceSettlementManager),settlementData);
                    Network.listener.EnqueuePacket(packet);

                    SaveManager.ForceSave();
                }
            }
        }
    }
}
