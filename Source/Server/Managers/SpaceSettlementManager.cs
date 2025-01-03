using GameServer.Core;
using GameServer.Managers;
using GameServer.Misc;
using GameServer.TCP;
using Shared;
using Shared.SOS2RTCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CommonEnumerators;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameServer.SOS2RTCompat
{
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
                    RemoveSpaceSettlement(client, spaceSettlementData);
                    break;
            }
        }

        public static void AddSpaceSettlement(ServerClient client, SpaceSettlementData settlementData) 
        {
            if (PlayerSettlementManager.CheckIfTileIsInUse(settlementData._settlementFile.Tile)) 
                ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Player {client.userFile.Label} attempted to add a ship at tile {settlementData._settlementFile.Tile}, but that tile already has a settlement");
            else
            {
                settlementData._settlementFile.UID = client.userFile.Uid;

                SpaceSettlementFile settlementFile = new SpaceSettlementFile();
                settlementFile.Tile = settlementData._settlementFile.Tile;
                settlementFile.UID = client.userFile.Uid;
                settlementFile.Phi = settlementData._phi;
                settlementFile.Radius = settlementData._radius;
                settlementFile.Theta = settlementData._theta;
                Serializer.SerializeToFile(Path.Combine(Master.settlementsPath, settlementFile.Tile + fileExtension), settlementFile);

                settlementData._stepMode = SettlementStepMode.Add;
                foreach (ServerClient cClient in NetworkHelper.GetConnectedClientsSafe())
                {
                    if (cClient == client) continue;
                    else
                    {
                        settlementData._settlementFile.Goodwill = GoodwillManager.GetSettlementGoodwill(cClient, settlementFile);

                        Packet rPacket = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName,settlementData);
                        cClient.listener.EnqueuePacket(rPacket);
                    }
                }
                Printer.Warning($"[SOS2][Added space settlement] > {settlementFile.Tile} > {client.userFile.Uid}");
            }
        }

        public static void RemoveSpaceSettlement(ServerClient client, SpaceSettlementData settlementData) 
        {
            if (!PlayerSettlementManager.CheckIfTileIsInUse(settlementData._settlementFile.Tile)) ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Ship at tile {settlementData._settlementFile.Tile} was attempted to be removed, but the tile doesn't contain a ship");

            SpaceSettlementFile settlementFile = GetSpaceSettlementFileFromTile(settlementData._settlementFile.Tile);

            if (client != null)
            {
                if (settlementFile.UID != client.userFile.Uid) ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Ship at tile {settlementData._settlementFile.Tile} attempted to be removed by {client.userFile.Uid}, but {settlementFile.UID} owns the ship");
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
                File.Delete(Path.Combine(Master.settlementsPath, settlementFile.Tile + fileExtension));

                Printer.Warning($"[SOS2][Remove ship] > {settlementFile.Tile}");
            }
            void SendRemovalSignal()
            {
                settlementData._stepMode = SettlementStepMode.Remove;

                Packet packet = Packet.CreatePacketFromObject(nameof(PlayerSettlementManager), settlementData);
                NetworkHelper.SendPacketToAllClients(packet, client);
            }
        }

        public static SpaceSettlementFile GetSpaceSettlementFileFromTile(int tileToGet)
        {
            string[] settlements = Directory.GetFiles(Master.settlementsPath);
            foreach (string settlement in settlements)
            {
                if (!settlement.EndsWith(fileExtension)) continue;

                SpaceSettlementFile settlementFile = Serializer.SerializeFromFile<SpaceSettlementFile>(settlement);
                if (settlementFile.Tile == tileToGet) return settlementFile;
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
