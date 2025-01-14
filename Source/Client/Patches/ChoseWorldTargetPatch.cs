using GameClient;
using GameClient.Managers;
using GameClient.Misc;
using GameClient.TCP;
using HarmonyLib;
using RimWorld.Planet;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using Verse;
using static Shared.CommonEnumerators;

namespace GameClient.SOS2RTCompat
{
    public static class ChoseWorldTargetPatch
    {
        private static int tile = -1;
        [HarmonyPatch(typeof(Building_ShipSensor), "ChoseWorldTarget")]
        public static class ChoseTargetPatch
        {
            [HarmonyPostfix]
            public static void DoPost(GlobalTargetInfo target, bool __result)
            {
                if (Network.state == ClientNetworkState.Connected)
                {
                    Printer.Warning($"[SOS2]Is observing {target.Tile}", LogImportanceMode.Verbose);
                    if (target.WorldObject == null && !Find.World.Impassable(target.Tile))
                    {
                        PlayerSettlementData settlementData = new PlayerSettlementData();
                        settlementData._settlementFile = new SettlementFile();
                        settlementData._settlementFile.Tile = target.Tile;
                        settlementData._stepMode = SettlementStepMode.Add;

                        Packet packet = Packet.CreatePacketFromObject(nameof(PlayerSettlementManager),settlementData);
                        Network.listener.EnqueuePacket(packet);

                        SaveManager.ForceSave();
                        tile = target.Tile;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(Building_ShipSensor), "PossiblyDisposeOfObservedMap")]
        public static class DeleteChosenTargetPatch
        {
            [HarmonyPostfix]
            public static void DoPost(Building_ShipSensor __instance)
            {
                if (Network.state == ClientNetworkState.Connected)
                {
                    Printer.Warning($"[SOS2]Has stopped observing {tile}", LogImportanceMode.Verbose);
                    if (tile != -1)
                    {
                        PlayerSettlementData settlementData = new PlayerSettlementData();
                        settlementData._settlementFile = new SpaceSettlementFile(-1);
                        settlementData._settlementFile.Tile = tile;
                        settlementData._stepMode = SettlementStepMode.Remove;

                        Packet packet = Packet.CreatePacketFromObject(nameof(PlayerSettlementManager), settlementData);
                        Network.listener.EnqueuePacket(packet);

                        SaveManager.ForceSave();
                    }
                }
                tile = -1;
            }
        }
    }
}
