using GameClient;
using GameClient.Managers;
using GameClient.Misc;
using GameClient.TCP;
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
                    Printer.Warning("[SOS2]Player lost ship.", LogImportanceMode.Verbose);
                    SpaceSettlementData data = new SpaceSettlementData();
                    data._stepMode = SettlementStepMode.Remove;
                    data._settlementFile = new SettlementFile();
                    data._settlementFile.Tile = Main.shipTile;
                    Main.shipTile = -1;

                    Packet packet = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName, data);
                    Network.listener.EnqueuePacket(packet);

                    SaveManager.ForceSave();
                }
            }
        }
    }
}
