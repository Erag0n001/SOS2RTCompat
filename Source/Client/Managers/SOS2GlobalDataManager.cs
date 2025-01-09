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
    [RTManager]
    public static class SOS2GlobalDataManager
    {
        public static SpaceSettlementData[] tempSettlements;
        public static void ParsePacket(Packet packet) 
        {
            GlobalData settlementList = Serializer.ConvertBytesToObject<GlobalData>(packet.contents);
            tempSettlements = settlementList._spaceSettlements;
        }
    }
}
