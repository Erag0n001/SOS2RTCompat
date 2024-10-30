using HarmonyLib;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.SOS2RTCompat.Patches
{
    public static class GlobalDataManagerPatch
    {
        [HarmonyPatch(typeof(GameServer.GlobalDataManagerHelper), "GetServerSettlements")]
        public static class GetAllSpaceSettlement
        {
            [HarmonyPostfix]
            public static void DoPost(ServerClient client, ServerGlobalData globalData)
            {
                GlobalDataManager.SendServerSpaceSettlements(client);
            }
        }
    }
}
