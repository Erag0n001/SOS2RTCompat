using Shared;
using GameServer;
using Shared.SOS2RTCompat;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameServer.SOSRTCompat
{
    public static class ShipMovementManager
    {
        public readonly static string fileExtension = ".mpspacesettlement";
        public static void HandlePacket(ServerClient client, Packet packet)
        {
            MovementData data = Serializer.ConvertBytesToObject<MovementData>(packet.contents);
            UpdateShip(client, data);
        }

        public static void UpdateShip(ServerClient client, MovementData data)
        {
            SpaceSettlementFile file = (SpaceSettlementFile)PlayerSettlementManager.GetSettlementFileFromTile(data._tile);
            if (file != null)
            {
                if (file.Owner == client.userFile.Username)
                {
                    file.Phi = data._phi;
                    file.Theta = data._theta;
                    file.Radius = data._radius;
                    Serializer.SerializeToFile(Path.Combine(Master.settlementsPath, data._tile + fileExtension), file);
                    Packet packet = Packet.CreateModdedPacketFromObject(nameof(ShipMovementManager), CommonValues.AssName, new MovementData() { _phi = file.Phi, _theta = file.Theta, _radius = file.Radius, _tile = file.Tile });
                    foreach (ServerClient gameClient in Network.connectedClients)
                    {
                        if (gameClient != client) gameClient.listener.EnqueuePacket(packet);
                    }
                        Logger.Warning($"[SOS2]{file.Owner}'s ship moved on tile {file.Tile} with coordinate:\nPhi:{file.Phi}, Theta:{file.Theta}, Radius:{file.Radius}",
                            CommonEnumerators.LogImportanceMode.Extreme);
                }
                else
                {
                    Logger.Error($"[SOS2]{client.userFile.Username} tried to move {file.Owner}'s ship at tile {file.Tile}");
                    Logger.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}",CommonEnumerators.LogImportanceMode.Verbose);
                }
            }
            else
            {
                Logger.Error($"[SOS2]Tried moving {client.userFile.Username}'s ship on tile {data._tile}, but it did not exist.");
                Logger.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}", CommonEnumerators.LogImportanceMode.Verbose);
            }
        }
    }
}
