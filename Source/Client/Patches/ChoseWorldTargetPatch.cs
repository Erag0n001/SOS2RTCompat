using GameClient;
using HarmonyLib;
using RimWorld.Planet;
using SaveOurShip2;
using Shared;
using Shared.SOS2RTCompat;
using Verse;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

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
                    Logger.Warning($"[SOS2]Is observing {target.Tile}", LogImportanceMode.Verbose);
                    if (target.WorldObject == null && !Find.World.Impassable(target.Tile))
                    {
                        PlayerSettlementData settlementData = new PlayerSettlementData();
                        settlementData._settlementData = new SettlementFile();
                        settlementData._settlementData.Tile = target.Tile;
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
                    Logger.Warning($"[SOS2]Has stopped observing {tile}", LogImportanceMode.Verbose);
                    if (tile != -1)
                    {
                        PlayerSettlementData settlementData = new PlayerSettlementData();
                        settlementData._settlementData = new SpaceSettlementFile();
                        settlementData._settlementData.Tile = tile;
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
