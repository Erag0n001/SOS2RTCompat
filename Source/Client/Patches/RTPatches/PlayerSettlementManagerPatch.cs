using HarmonyLib;
using Shared;

namespace GameClient.SOS2RTCompat
{
    public static class PlayerSettlementManagerPatch
    {
        [HarmonyPatch(typeof(PlayerSettlementManager), "SpawnSingleSettlement")]
        public static class CheckIfSpaceSettlementExist
        {
            [HarmonyPrefix]
            public static bool DoPre(SettlementFile toAdd)
            {
                WorldObjectFakeOrbitingShip obj = WorldObjectManager.FindWorldObjectFromTile<WorldObjectFakeOrbitingShip>(toAdd.Tile);
                if (obj != null) 
                {
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(PlayerSettlementManager), "SendNewPlayerSettlement")]
        public static class CheckIfSpaceSettlementExistInit
        {
            [HarmonyPrefix]
            public static bool DoPre(int settlementTile)
            {
                WorldObjectFakeOrbitingShip obj = WorldObjectManager.FindWorldObjectFromTile<WorldObjectFakeOrbitingShip>(settlementTile);
                if (obj != null)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
