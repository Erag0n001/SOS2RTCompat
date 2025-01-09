using GameServer.Managers;
using GameServer.TCP;
using Shared;
using Shared.SOS2RTCompat;
using static Shared.CommonEnumerators;

namespace GameServer.SOS2RTCompat
{
    [RTManager]
    public static class SpaceGoodwillManager
    {
        public static void ParsePacket(ServerClient client, Packet packet)
        {
            FactionGoodwillData data = Serializer.ConvertBytesToObject<FactionGoodwillData>(packet.contents);
            ChangeUserGoodwills(client, data);
        }

        public static void ChangeUserGoodwills(ServerClient client, FactionGoodwillData data)
        {
            SpaceSettlementFile settlementFile = SpaceSettlementManager.GetSpaceSettlementFileFromTile(data._tile);

            data._uid = settlementFile.UID;

            if (GuildManagerH.GetFactionFromFactionName(client.userFile.GuildName).CurrentUids.Contains(data._uid))
            {
                ResponseShortcutManager.SendBreakPacket(client);
                return;
            }

            client.userFile.EnemyPlayers.Remove(data._uid);
            client.userFile.AllyPlayers.Remove(data._uid);

            if (data._goodwill == Goodwill.Enemy)
            {
                if (!client.userFile.EnemyPlayers.Contains(data._uid))
                {
                    client.userFile.EnemyPlayers.Add(data._uid);
                }
            }

            else if (data._goodwill == Goodwill.Ally)
            {
                if (!client.userFile.AllyPlayers.Contains(data._uid))
                {
                    client.userFile.AllyPlayers.Add(data._uid);
                }
            }

            List<Goodwill> tempSettlementList = new List<Goodwill>();
            SpaceSettlementFile[] settlements = SpaceSettlementManager.GetAllSettlements();
            foreach (SpaceSettlementFile settlement in settlements)
            {
                //Check if settlement owner is the one we are looking for

                if (settlement.UID == data._uid)
                {
                    data._settlementTiles.Add(settlement.Tile);
                    tempSettlementList.Add(GetSettlementGoodwill(client, settlement));
                }
            }
            data._settlementGoodwills = tempSettlementList.ToArray();

            UserManagerH.SaveUserFile(client.userFile);

            Packet rPacket = Packet.CreatePacketFromObject(nameof(SpaceGoodwillManager), data);
            client.listener.EnqueuePacket(rPacket);
        }

        public static Goodwill GetSettlementGoodwill(ServerClient client, SpaceSettlementFile settlement)
        {
            if (GuildManagerH.GetFactionFromFactionName(client.userFile.GuildName).CurrentUids.Contains(settlement.UID))
            {
                if (settlement.UID == client.userFile.Uid) return Goodwill.Personal;
                else return Goodwill.Faction;
            }

            else if (client.userFile.EnemyPlayers.Contains(settlement.UID)) return Goodwill.Enemy;
            else if (client.userFile.AllyPlayers.Contains(settlement.UID)) return Goodwill.Ally;
            else if (settlement.UID == client.userFile.Uid) return Goodwill.Personal;
            else return Goodwill.Neutral;
        }
    }
}
