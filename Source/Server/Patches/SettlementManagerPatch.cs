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
            public static void DoPost(int tileToCheck, bool __result)
            {
                string[] settlements = Directory.GetFiles(Master.settlementsPath);
                foreach (string settlement in settlements)
                {
                    if (!settlement.EndsWith(SpaceSettlementManager.fileExtension)) continue;

                    SettlementFile settlementJSON = Serializer.SerializeFromFile<SettlementFile>(settlement);
                    if (settlementJSON.Tile == tileToCheck)
                    {
                        __result = true;
                        break;
                    }
                }
            }
        }
    }
}
