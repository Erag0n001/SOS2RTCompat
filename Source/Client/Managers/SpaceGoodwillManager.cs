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
    [RTManager]
    public static class SpaceGoodwillManager
    {
        public static void ParsePacket(Packet packet)
        {
            SpaceFactionGoodwillData factionGoodwillData = Serializer.ConvertBytesToObject<SpaceFactionGoodwillData>(packet.contents);
            SpaceSettlementManager.ChangeGoodwill(factionGoodwillData);
            DialogManager.PopWaitDialog();
        }

        public static void TryRequestGoodwill(Goodwill type, GoodwillTarget target)
        {
            int tileToUse = SOS2SessionValues.chosenWorldObject.Tile;

            Faction factionToUse = SOS2SessionValues.chosenWorldObject.Faction;

            if (type == Goodwill.Enemy)
            {
                if (factionToUse == FactionValues.enemyPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as enemy!" });
                    DialogManager.PushNewDialog(d1);
                }
                else RequestChangeStructureGoodwill(tileToUse, Goodwill.Enemy);
            }
            else if (type == Goodwill.Neutral)
            {
                if (factionToUse == FactionValues.neutralPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as neutral!" });
                    DialogManager.PushNewDialog(d1);
                }
                else RequestChangeStructureGoodwill(tileToUse, Goodwill.Neutral);
            }

            else if (type == Goodwill.Ally)
            {
                if (factionToUse == FactionValues.allyPlayer)
                {
                    RT_Dialog_Message d1 = new RT_Dialog_Message("ERROR", new string[] { "Chosen ship is already marked as ally!" });
                    DialogManager.PushNewDialog(d1);
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
            Packet packet = Packet.CreatePacketFromObject(nameof(SpaceGoodwillManager), factionGoodwillData);
            Network.listener.EnqueuePacket(packet);

            RT_Dialog_Wait d1 = new RT_Dialog_Wait("Changing ship goodwill");
            DialogManager.PushNewDialog(d1);
        }
    }
}
