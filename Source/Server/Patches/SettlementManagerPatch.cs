using GameServer;
using HarmonyLib;
using Shared;
using Shared.SOS2RTCompat;

namespace GameServer.SOS2RTCompat
{
    public static class PlayerSettlementManagerPatch
    {
        [HarmonyPatch(typeof(PlayerSettlementManager), "CheckIfTileIsInUse")]
        public static class CheckIfSpaceSettlementExist
        {
            [HarmonyPostfix]
            public static void DoPost(bool __result, int tile)
            {
                string[] settlements = Directory.GetFiles(Master.settlementsPath);
                foreach (string settlement in settlements)
                {
                    if (!settlement.EndsWith(SpaceSettlementManager.fileExtension)) continue;

                    SettlementFile settlementJSON = Serializer.SerializeFromFile<SettlementFile>(settlement);
                    if (settlementJSON.Tile == tile)
                    {
                        __result = true;
                        break;
                    }
                }
            }
        }
        [HarmonyPatch(typeof(PlayerSettlementManager), "GetSettlementFileFromTile")]
        public static class GetSettlementFromTile
        {
            [HarmonyPostfix]
            public static void DoPost(SettlementFile __result, int tileToGet)
            {
                if (__result != null) return;
                string[] settlements = Directory.GetFiles(Master.settlementsPath);
                foreach (string settlement in settlements)
                {
                    if (!settlement.EndsWith(SpaceSettlementManager.fileExtension)) continue;
                    SpaceSettlementFile settlementFile = (Serializer.SerializeFromFile<SpaceSettlementFile>(settlement));
                    if (settlementFile.Tile == tileToGet)
                    {
                        __result = (SettlementFile)settlementFile;
                        return;
                    }
                }
                __result = null;
            }
        }
        [HarmonyPatch(typeof(PlayerSettlementManager), "GetAllSettlements")]
        public static class GetAllSettlements
        {
            [HarmonyPostfix]
            public static void DoPost(SettlementFile[] __result)
            {
                List<SettlementFile> settlementList = __result.ToList();

                string[] settlements = Directory.GetFiles(Master.settlementsPath);
                foreach (string settlement in settlements)
                {
                    if (!settlement.EndsWith(SpaceSettlementManager.fileExtension)) continue;
                    settlementList.Add(Serializer.SerializeFromFile<SettlementFile>(settlement));
                }
                __result = settlementList.ToArray();
            }
        }
        [HarmonyPatch(typeof(PlayerSettlementManager), "GetAllSettlementsFromUsername")]
        public static class GetAllSettlementsFromUserName
        {
            [HarmonyPostfix]
            public static void DoPost(SettlementFile[] __result, string usernameToCheck)
            {
                List<SettlementFile> settlementList = __result.ToList();

                string[] settlements = Directory.GetFiles(Master.settlementsPath);
                foreach (string settlement in settlements)
                {
                    if (!settlement.EndsWith(SpaceSettlementManager.fileExtension)) continue;
                    SpaceSettlementFile settlementFile = (Serializer.SerializeFromFile<SpaceSettlementFile>(settlement));
                    if(settlementFile.Owner == usernameToCheck)
                        settlementList.Add((SettlementFile)settlementFile);
                }
                __result = settlementList.ToArray();
            }
        }
    }
}
