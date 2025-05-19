using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameClient.Dialogs;
using GameClient.Managers;
using GameClient.TCP;
using GameClient.Values;
using RimWorld;
using Shared;
using static Shared.CommonEnumerators;

namespace GameClient.SOS2RTCompat
{
    public static class SpaceGoodwillManager
    {
        [HandlesModdedPacket(ModdedPacketTypes.ShipGoodwill)]
        public static void ParsePacket(byte[] packet)
        {
            SpaceFactionGoodwillData factionGoodwillData = Serializer.ConvertBytesToObject<SpaceFactionGoodwillData>(packet);
            SpaceSettlementManager.ChangeGoodwill(factionGoodwillData);
            RT_Dialog_Base.PushNewDialog(new RT_Dialog_Wait("Waiting for server"));
        }

        public static void TryRequestGoodwill(Goodwill type, GoodwillTarget target)
        {
            int tileToUse = SOS2SessionValues.chosenWorldObject.Tile;

            Faction factionToUse = SOS2SessionValues.chosenWorldObject.Faction;

            if (type == Goodwill.Enemy)
            {
                if (factionToUse == ClientValues.EnemyPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as enemy!" });
                    RT_Dialog_Base.PushNewDialog(d1);
                }
                else RequestChangeStructureGoodwill(tileToUse, Goodwill.Enemy);
            }
            else if (type == Goodwill.Neutral)
            {
                if (factionToUse == ClientValues.NeutralPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as neutral!" });
                    RT_Dialog_Base.PushNewDialog(d1);
                }
                else RequestChangeStructureGoodwill(tileToUse, Goodwill.Neutral);
            }

            else if (type == Goodwill.Ally)
            {
                if (factionToUse == ClientValues.AllyPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as ally!" });
                    RT_Dialog_Base.PushNewDialog(d1);
                }
                else RequestChangeStructureGoodwill(tileToUse, Goodwill.Ally);
            }
        }

        public static void RequestChangeStructureGoodwill(int structureTile, Goodwill goodwill)
        {
            SpaceFactionGoodwillData factionGoodwillData = new SpaceFactionGoodwillData(
                (SOS2SessionValues.chosenWorldObject as WorldObjectFakeOrbitingShip).serverId);
            factionGoodwillData._tile = structureTile;
            factionGoodwillData._goodwill = goodwill;
            Network.Listener.EnqueueModdedPacket(ModdedPacketTypes.ShipGoodwill, factionGoodwillData);

            RT_Dialog_Wait d1 = new RT_Dialog_Wait("Changing ship goodwill");
            RT_Dialog_Base.PushNewDialog(d1);
        }
    }
}
