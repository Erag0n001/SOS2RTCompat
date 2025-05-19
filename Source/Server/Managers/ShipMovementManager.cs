using Shared;
using Shared.SOS2RTCompat;
using GameServer.TCP;
using GameServer.Core;
using GameServer.Misc;
using GameServer.SOS2RTCompat;

namespace GameServer.SOSRTCompat
{
    [RTManager]
    public static class ShipMovementManager
    {
        public readonly static string fileExtension = ".mpship";
        [HandlesModdedPacket(ModdedPacketTypes.ShipMovement)]
        public static void ParsePacket(ServerClient client, byte[] packet)
        {
            MovementData data = Serializer.ConvertBytesToObject<MovementData>(packet);
            UpdateShip(client, data);
        }

        public static void UpdateShip(ServerClient client, MovementData data)
        {
            SpaceSettlementFile file = SpaceSettlementManager.GetSettlementFromUID(client.UserFile.Uid);
            if (file != null)
            {
                if (file.UID == client.UserFile.Uid)
                {
                    file.Phi = data._phi;
                    file.Theta = data._theta;
                    file.Radius = data._radius;
                    Serializer.SerializeToFile(Path.Combine(Master.SettlementsPath, client.UserFile.Uid + fileExtension), file);
                    MovementData dataForClients = new MovementData(file.ID);
                    dataForClients._phi = file.Phi;
                    dataForClients._theta = file.Theta;
                    dataForClients._radius = file.Radius;
                    dataForClients._tile = file.Tile;
                    foreach (ServerClient gameClient in Network.ConnectedClients)
                    {
                        if (gameClient != client) gameClient.Listener.EnqueueModdedPacket(ModdedPacketTypes.ShipMovement, dataForClients);
                    }
                    Printer.Warning($"[SOS2]{file.UID}'s ship moved with id {file.ID} with coordinate:\nPhi:{file.Phi}, Theta:{file.Theta}, Radius:{file.Radius}",
                        CommonEnumerators.LogImportanceMode.Extreme);
                }
                else
                {
                    Printer.Error($"[SOS2]{client.UserFile.Uid} tried to move {file.UID}'s ship with id {file.ID}");
                    Printer.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}", CommonEnumerators.LogImportanceMode.Verbose);
                }
            }
            else
            {
                Printer.Error($"[SOS2]Tried moving {client.UserFile.Uid}'s ship with id {data.shipID}, but it did not exist.");
                Printer.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}", CommonEnumerators.LogImportanceMode.Verbose);
            }
        }
    }
}
