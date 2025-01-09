using GameServer.Core;
using GameServer.Managers;
using GameServer.Misc;
using GameServer.TCP;
using Shared;
using Shared.SOS2RTCompat;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Shared.CommonEnumerators;

namespace GameServer.SOS2RTCompat
{
    [RTManager]
    public static class SpaceSettlementManager
    {
        public readonly static string fileExtension = ".mpship";
        public static void ParsePacket(ServerClient client, Packet packet) 
        {
            SpaceSettlementData spaceSettlementData = Serializer.ConvertBytesToObject<SpaceSettlementData>(packet.contents);
            switch (spaceSettlementData._stepMode)
            {
                case CommonEnumerators.SettlementStepMode.Add:
                    AddSpaceSettlement(client, spaceSettlementData);
                    break;
                case CommonEnumerators.SettlementStepMode.Remove:
                    RemoveSpaceSettlement(client, spaceSettlementData._settlementFile.Tile);
                    break;
            }
        }

        public static void AddSpaceSettlement(ServerClient client, SpaceSettlementData settlementData) 
        {
            settlementData._settlementFile.UID = client.userFile.Uid;

            SpaceSettlementFile settlementFile = new SpaceSettlementFile();
            settlementFile.Tile = settlementData._settlementFile.Tile;
            settlementFile.UID = client.userFile.Uid;
            settlementFile.Label = client.userFile.Label;
            settlementFile.Phi = settlementData._settlementFile.Phi;
            settlementFile.Radius = settlementData._settlementFile.Radius;
            settlementFile.Theta = settlementData._settlementFile.Theta;
            Serializer.SerializeToFile(Path.Combine(Master.settlementsPath, settlementFile.UID + fileExtension), settlementFile);

            settlementData._stepMode = SettlementStepMode.Add;
            foreach (ServerClient cClient in NetworkHelper.GetConnectedClientsSafe())
            {
                if (cClient == client) continue;
                else
                {
                    settlementData._settlementFile.Goodwill = GoodwillManager.GetSettlementGoodwill(cClient, settlementFile);

                    Packet rPacket = Packet.CreatePacketFromObject(nameof(SpaceSettlementManager),settlementData);
                    cClient.listener.EnqueuePacket(rPacket);
                }
            }
            Printer.Warning($"[SOS2][Added space settlement] > {settlementFile.Tile} > {client.userFile.Uid}");
        }

        public static void RemoveSpaceSettlement(ServerClient client, int tile) 
        {
            if (!PlayerSettlementManager.CheckIfTileIsInUse(tile))
                ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Ship at tile {tile} was attempted to be removed, but the tile doesn't contain a ship");

            SpaceSettlementFile settlementFile = GetSpaceSettlementFileFromTile(tile);

            if (client != null)
            {
                if (settlementFile.UID != client.userFile.Uid)
                    ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Ship at tile {tile} attempted to be removed by {client.userFile.Uid}, but {settlementFile.UID} owns the ship");
                else
                {
                    Delete();
                    SendRemovalSignal();
                }
            }
            else
            {
                Delete();
                SendRemovalSignal();
            }
            void Delete()
            {
                File.Delete(Path.Combine(Master.settlementsPath, settlementFile.UID + fileExtension));

                Printer.Warning($"[SOS2][Remove ship] > {settlementFile.Tile}");
            }
            void SendRemovalSignal()
            {
                SpaceSettlementData data = new SpaceSettlementData()
                {
                    _settlementFile = settlementFile,
                    _stepMode = SettlementStepMode.Remove
                };

                Packet packet = Packet.CreatePacketFromObject(nameof(PlayerSettlementManager), data);
                NetworkHelper.SendPacketToAllClients(packet, client);
            }
        }

        public static SpaceSettlementFile GetSettlementFromUID(string UID) 
        {
            SpaceSettlementFile[] spaceSettlements = GetAllSettlements();
            foreach (SpaceSettlementFile settlement in spaceSettlements)
            {
                if (settlement.UID == UID)
                    return settlement;
            }
            return null;
        }
        public static SpaceSettlementFile GetSpaceSettlementFileFromTile(int tileToGet)
        {
            SpaceSettlementFile[] spaceSettlements = GetAllSettlements();
            foreach (SpaceSettlementFile settlement in spaceSettlements)
            {
                if (settlement.Tile == tileToGet) 
                    return settlement;
            }
            return null;
        }

        public static SpaceSettlementFile[] GetAllSettlements()
        {
            List<SpaceSettlementFile> settlementList = new List<SpaceSettlementFile>();

            string[] settlements = Directory.GetFiles(Master.settlementsPath);
            foreach (string settlement in settlements)
            {
                if (!settlement.EndsWith(fileExtension)) continue;
                settlementList.Add(Serializer.SerializeFromFile<SpaceSettlementFile>(settlement));
            }

            return settlementList.ToArray();
        }
    }
}
