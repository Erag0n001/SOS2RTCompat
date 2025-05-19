using Shared;
using System.Threading;
using System.Threading.Tasks;
using Shared.SOS2RTCompat;
using GameClient.TCP;
using GameClient.Misc;
namespace GameClient.SOS2RTCompat
{
    public static class ShipMovementManager
    {
        public static bool shipMoved = false;
        public static float phi;
        public static float theta;
        public static float radius;
        public static int tile = -1;
        private const int sleepTime = 250;
        static ShipMovementManager()
        {
            Task.Run(PositionChecker);
        }
        [HandlesModdedPacket(ModdedPacketTypes.ShipMovement)]
        public static void ParsePacket(byte[] packet) 
        {
            MovementData data = Serializer.ConvertBytesToObject<MovementData>(packet);
            MoveShipFromTile(data);
        }
        public static void PositionChecker()
        {
            while (true)
            {
                Thread.Sleep(sleepTime);
                if (shipMoved)
                {
                    var data = new MovementData(-1) {
                        _phi = phi, 
                        _theta = theta, 
                        _radius = radius, 
                        _tile = tile };
                    Network.Listener.EnqueueModdedPacket(ModdedPacketTypes.ShipMovement, data);
                    shipMoved = false;
                }
            }

        }
        public static void MoveShipFromTile(MovementData data)
        {
            WorldObjectFakeOrbitingShip ship = SpaceSettlementManager.spacePlayerSettlement.Find(x => x.Tile == data._tile);
            ship.phi = data._phi;
            ship.theta = data._theta;
            ship.radius = data._radius;
            ship.OrbitSet();
        }
    }
}
