using GameClient.Dialogs;
using GameClient.Managers;
using GameClient.Values;
using RimWorld;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CommonEnumerators;

namespace GameClient.SOS2RTCompat
{
    [RTManager]
    public class ShipGoodwillManager
    {
        public static void TryRequestGoodwill(Goodwill type)
        {
            int tileToUse = SOS2SessionValues.chosenWorldObject.Tile;
            Faction factionToUse = factionToUse = SOS2SessionValues.chosenWorldObject.Faction;

            if (type == Goodwill.Enemy)
            {
                if (factionToUse == FactionValues.enemyPlayer)
                {
                    RT_Dialog_Error d1 = new RT_Dialog_Error("Chosen settlement is already marked as enemy!");
                    DialogManager.PushNewDialog(d1);
                }
                else SpaceSettlementManager.ChangeGoodwill(tileToUse, Goodwill.Enemy);
            }

            else if (type == Goodwill.Neutral)
            {
                if (factionToUse == FactionValues.neutralPlayer)
                {
                    RT_Dialog_Error d1 = new RT_Dialog_Error("Chosen settlement is already marked as neutral!");
                    DialogManager.PushNewDialog(d1);
                }
                else SpaceSettlementManager.ChangeGoodwill(tileToUse, Goodwill.Neutral);
            }

            else if (type == Goodwill.Ally)
            {
                if (factionToUse == FactionValues.allyPlayer)
                {
                    RT_Dialog_Error d1 = new RT_Dialog_Error("Chosen settlement is already marked as ally!");
                    DialogManager.PushNewDialog(d1);
                }
                else SpaceSettlementManager.ChangeGoodwill(tileToUse, Goodwill.Ally);
            }
        }
    }
}