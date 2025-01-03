using GameClient;
using GameClient.TCP;
using HarmonyLib;
using SaveOurShip2;
using static Shared.CommonEnumerators;

namespace GameClient.SOS2RTCompat
{
    [HarmonyPatch(typeof(WorldObjectOrbitingShip), nameof(WorldObjectOrbitingShip.Tick))]
    public static class ShipMovementPatch
    {
        [HarmonyPostfix]
        public static void DoPost(WorldObjectOrbitingShip __instance)
        {
            if (Network.state == ClientNetworkState.Connected)
            {
                if (__instance.orbitalMove != 0)
                {
                    ShipMovementManager.phi = __instance.Phi;
                    ShipMovementManager.theta = __instance.Theta;
                    ShipMovementManager.radius = __instance.Radius;
                    if (ShipMovementManager.tile == -1)
                    {
                        ShipMovementManager.tile = __instance.Map.Tile;
                    }
                    ShipMovementManager.shipMoved = true;
                }
            }
        }
    }
}
