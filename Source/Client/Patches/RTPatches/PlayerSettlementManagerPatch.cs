using HarmonyLib;
using Shared;
using SaveOurShip2;
using GameClient.Managers;
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
                WorldObjectOrbitingShip obj = SOS2WorldObjectManager.FindWorldObjectFromTile<WorldObjectOrbitingShip>(toAdd.Tile);
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
                WorldObjectOrbitingShip obj = SOS2WorldObjectManager.FindWorldObjectFromTile<WorldObjectOrbitingShip>(settlementTile);
                if (obj != null)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
