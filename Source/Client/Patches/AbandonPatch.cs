using GameClient;
using HarmonyLib;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(WorldObjectOrbitingShip), nameof(WorldObjectOrbitingShip.Abandon))]
    public static class ShipAbandonPatch
    {
        [HarmonyPostfix]
        public static void DoPost()
        {
            if (Network.state == ClientNetworkState.Connected)
            {
                Logger.Warning("[SOS2]Player abandoned ship.", LogImportanceMode.Verbose);
                PlayerSettlementData settlementData = new PlayerSettlementData();
                settlementData._settlementData = new SpaceSettlementFile();
                settlementData._settlementData.Tile = Main.shipTile;
                Main.shipTile = -1;
                settlementData._stepMode = SettlementStepMode.Remove;

                Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName, settlementData);
                Network.listener.EnqueuePacket(packet);

                SaveManager.ForceSave();
            }
        }
    }
}
