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
        [HandlesModdedPacket(ModdedPacketTypes.ShipGoodwill)]
        public static void ParsePacket(ServerClient client, byte[] packet)
        {
            SpaceFactionGoodwillData data = Serializer.ConvertBytesToObject<SpaceFactionGoodwillData>(packet);
            ChangeUserGoodwills(client, data);
        }

        public static void ChangeUserGoodwills(ServerClient client, SpaceFactionGoodwillData data)
        {
            SpaceSettlementFile settlementFile = SpaceSettlementManager.GetSettlementFromID(client, data._id);

            data._uid = settlementFile.UID;

            if (GuildManagerH.GetFactionFromFactionName(client.UserFile.GuildName).CurrentUids.Contains(data._uid))
            {
                ResponseShortcutManager.SendBreakPacket(client);
                return;
            }

            client.UserFile.EnemyPlayers.Remove(data._uid);
            client.UserFile.AllyPlayers.Remove(data._uid);

            if (data._goodwill == Goodwill.Enemy)
            {
                if (!client.UserFile.EnemyPlayers.Contains(data._uid))
                {
                    client.UserFile.EnemyPlayers.Add(data._uid);
                }
            }

            else if (data._goodwill == Goodwill.Ally)
            {
                if (!client.UserFile.AllyPlayers.Contains(data._uid))
                {
                    client.UserFile.AllyPlayers.Add(data._uid);
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

            UserManagerH.SaveUserFile(client.UserFile);

            client.Listener.EnqueueModdedPacket(ModdedPacketTypes.ShipGoodwill, data);
        }

        public static Goodwill GetSettlementGoodwill(ServerClient client, SpaceSettlementFile settlement)
        {
            if (GuildManagerH.GetFactionFromFactionName(client.UserFile.GuildName).CurrentUids.Contains(settlement.UID))
            {
                if (settlement.UID == client.UserFile.Uid) return Goodwill.Personal;
                else return Goodwill.Faction;
            }

            else if (client.UserFile.EnemyPlayers.Contains(settlement.UID)) return Goodwill.Enemy;
            else if (client.UserFile.AllyPlayers.Contains(settlement.UID)) return Goodwill.Ally;
            else if (settlement.UID == client.UserFile.Uid) return Goodwill.Personal;
            else return Goodwill.Neutral;
        }
    }
}
