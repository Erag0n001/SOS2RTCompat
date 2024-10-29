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
        public readonly static string fileExtension = ".mpspacesettlement";
        public static void ParsePacket(ServerClient client, Packet packet) 
        {
            SpaceSettlementData spaceSettlementData = Serializer.ConvertBytesToObject<SpaceSettlementData>(packet.contents);
            switch (spaceSettlementData._stepMode)
            {
                case CommonEnumerators.SettlementStepMode.Add:
                    AddSpaceSettlement(client, spaceSettlementData);
                    break;
                case CommonEnumerators.SettlementStepMode.Remove:
                    break;
            }
        }

        public static void AddSpaceSettlement(ServerClient client, SpaceSettlementData settlementData) 
        {
            if (PlayerSettlementManager.CheckIfTileIsInUse(settlementData._settlementData.Tile)) ResponseShortcutManager.SendIllegalPacket(client, $"[SOS2]Player {client.userFile.Username} attempted to add a ship at tile {settlementData._settlementData.Tile}, but that tile already has a settlement");
            else
            {
                settlementData._settlementData.Owner = client.userFile.Username;

                SpaceSettlementFile settlementFile = new SpaceSettlementFile();
                settlementFile.Tile = settlementData._settlementData.Tile;
                settlementFile.Owner = client.userFile.Username;
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
                        settlementData._settlementData.Goodwill = GoodwillManager.GetSettlementGoodwill(cClient, settlementFile);

                        Packet rPacket = Packet.CreateModdedPacketFromObject(nameof(SpaceSettlementManager), CommonValues.AssName,settlementData);
                        cClient.listener.EnqueuePacket(rPacket);
                    }
                }

                Logger.Warning($"[SOS2][Added space settlement] > {settlementFile.Tile} > {client.userFile.Username}");
            }
        }
    }
}
