using GameClient;
using HarmonyLib;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(WorldObjectOrbitingShip), nameof(WorldObjectOrbitingShip.ShouldRemoveMapNow))]
    public static class ShipLostAndCrashing
    {
        [HarmonyPostfix]
        public static void DoPost(WorldObjectOrbitingShip __instance)
        {
            if (Network.state == ClientNetworkState.Connected)
            {
                if (__instance.Map.GetComponent<ShipMapComp>().ShipMapState == ShipMapState.burnUpSet)
                {
                    Logger.Warning("[SOS2]Player lost ship.", LogImportanceMode.Verbose);
                    SpaceSettlementData data = new SpaceSettlementData();
                    data._stepMode = SettlementStepMode.Remove;
                    data._settlementData = new SettlementFile();
                    data._settlementData.Tile = Main.shipTile;
                    Main.shipTile = -1;

                    Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName, data);
                    Network.listener.EnqueuePacket(packet);

                    SaveManager.ForceSave();
                }
            }
        }
    }
}
