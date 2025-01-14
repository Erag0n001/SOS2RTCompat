using GameServer.Core;
using GameServer.Managers;
using GameServer.TCP;
using HarmonyLib;
using Shared;

namespace GameServer.SOS2RTCompat
{
    public static class SaveManagerPatch
    {
        [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.ResetPlayerData))]
        public static class CheckIfSpaceSettlementExist
        {
            [HarmonyPostfix]
            public static void DoPost(ServerClient client, string uid)
            {
                SpaceSettlementManager.RemoveSpaceSettlement(client, SpaceSettlementManager.GetSettlementFromUID(uid));
            }
        }
    }
}
