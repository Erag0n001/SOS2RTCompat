using Shared;
using System.Threading;
using System.Threading.Tasks;
using Shared.SOS2RTCompat;
using GameClient.TCP;
namespace GameClient.SOS2RTCompat
{
    [RTManager]
    public static class ShipMovementManager
    {
        public static bool shipMoved = false;
        public static float phi;
        public static float theta;
        public static float radius;
        public static int tile = -1;
        private static readonly int sleepTime = 125;
        static ShipMovementManager()
        {
            Task.Run(PositionChecker);
        }
        public static void ParsePacket(Packet packet) 
        {

        }
        public static void PositionChecker()
        {
            while (true)
            {
                Thread.Sleep(sleepTime);
                if (shipMoved)
                {
                    Packet packet = Packet.CreatePacketFromObject(nameof(ShipMovementManager), new MovementData() { _phi = phi, _theta = theta, _radius = radius, _tile = tile });
                    Network.listener.EnqueuePacket(packet);
                    shipMoved = false;
                }
            }

        }
        public static void MoveShipFromTile(Packet data)
        {
            MovementData movement = Serializer.ConvertBytesToObject<MovementData>(data.contents);
            WorldObjectFakeOrbitingShip ship = SpaceSettlementManager.spacePlayerSettlement.Find(x => x.Tile == movement._tile);
            ship.phi = movement._phi;
            ship.theta = movement._theta;
            ship.radius = movement._radius;
            ship.OrbitSet();
        }
    }
}
