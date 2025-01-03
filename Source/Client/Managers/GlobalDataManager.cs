using GameClient.Misc;
using Shared;
using Shared.SOS2RTCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient.SOS2RTCompat
{
    public static class GlobalDataManager
    {
        public static void ParsePacket(Packet packet) 
        {
            GlobalData settlementList = Serializer.ConvertBytesToObject<GlobalData>(packet.contents);
            foreach (SpaceSettlementData settlement in settlementList._spaceSettlements)
            {
                Printer.Warning("Test");
                SpaceSettlementManager.SpawnSingleSettlement(settlement);
            }
        }
    }
}
