using Shared;
using GameServer;
using Shared.SOS2RTCompat;
using CommonValues = Shared.SOS2RTCompat.CommonValues;
using GameServer.TCP;
using GameServer.Managers;
using GameServer.Core;
using GameServer.Misc;

namespace GameServer.SOSRTCompat
{
    [RTManager]
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
                if (file.UID == client.userFile.Uid)
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
                        Printer.Warning($"[SOS2]{file.UID}'s ship moved on tile {file.Tile} with coordinate:\nPhi:{file.Phi}, Theta:{file.Theta}, Radius:{file.Radius}",
                            CommonEnumerators.LogImportanceMode.Extreme);
                }
                else
                {
                    Printer.Error($"[SOS2]{client.userFile.Uid} tried to move {file.UID}'s ship at tile {file.Tile}");
                    Printer.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}",CommonEnumerators.LogImportanceMode.Verbose);
                }
            }
            else
            {
                Printer.Error($"[SOS2]Tried moving {client.userFile.Uid}'s ship on tile {data._tile}, but it did not exist.");
                Printer.Error($"[SOS2]Debugging information:\n{StringUtilities.ToString(data)}", CommonEnumerators.LogImportanceMode.Verbose);
            }
        }
    }
}
