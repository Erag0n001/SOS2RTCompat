using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClient.Core;
using GameClient.Managers;
using HarmonyLib;
using SaveOurShip2;
using Shared.SOS2RTCompat;

namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(PlanetManager), "BuildPlanet")]
    public static class BuildShips
    {
        [HarmonyPostfix]
        public static void DoPost()
        {
            Master.threadDispatcher.Enqueue(() =>
            {
                SpaceSettlementManager.ClearAllSettlements();
                foreach (SpaceSettlementData data in SOS2GlobalDataManager.tempSettlements) 
                {
                    SpaceSettlementManager.SpawnSingleSettlement(data);
                }
            });
        }
    }
}
