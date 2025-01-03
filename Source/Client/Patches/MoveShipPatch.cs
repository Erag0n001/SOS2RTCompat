using HarmonyLib;
using Verse;
using Shared;
using static Shared.CommonEnumerators;
using GameClient;
using SaveOurShip2;
using RimWorld.Planet;
using System.Linq;
using Shared.SOS2RTCompat;
using CommonValues = Shared.SOS2RTCompat.CommonValues;
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
                GameParameterManager.SetDifficulty(GameParameterManager.GetDifficulty());
                Map map = core.Map;
                if (!targetMap.IsSpace() && ShipInteriorMod2.FindPlayerShipMap() == null)
                {
                    Printer.Warning("[SOS2]Deleting empty space map", LogImportanceMode.Verbose);
                    PlayerSettlementData settlementData = new PlayerSettlementData();
                    settlementData._settlementFile = new SpaceSettlementFile();
                    settlementData._settlementFile.Tile = Main.shipTile;
                    Main.shipTile = -1;
                    settlementData._stepMode = SettlementStepMode.Remove;

                    Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName,settlementData);
                    Network.listener.EnqueuePacket(packet);

                    SaveManager.ForceSave();
                }
            }
        }
    }
}
