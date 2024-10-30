using Shared;
using Shared.SOS2RTCompat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonValues = Shared.SOS2RTCompat.CommonValues;

namespace GameServer.SOS2RTCompat
{
    public static class GlobalDataManager
    {
        public static void SendServerSpaceSettlements(ServerClient client)
        {
            List<SpaceSettlementData> tempList = new List<SpaceSettlementData>();
            SpaceSettlementFile[] settlements = SpaceSettlementManager.GetAllSettlements();
            foreach (SpaceSettlementFile settlement in settlements)
            {
                SpaceSettlementData data = new SpaceSettlementData();

                if (settlement.Owner == client.userFile.Username) continue;
                else
                {
                    data._settlementData = new SettlementFile()
                    {
                        Owner = client.userFile.Username,
                        Tile = settlement.Tile,
                        Goodwill = settlement.Goodwill,
                    };
                    data._phi = settlement.Phi;
                    data._theta = settlement.Theta;
                    data._radius = settlement.Radius;
                    data._stepMode = CommonEnumerators.SettlementStepMode.Add;

                    tempList.Add(data);
                }
            }
            GlobalData global = new GlobalData() { _spaceSettlements = tempList.ToArray()};
            Logger.Warning("Test");
            Packet packet = Packet.CreateModdedPacketFromObject(nameof(GlobalDataManager), CommonValues.AssName, global);
            client.listener.EnqueuePacket(packet);
        }
    }
}
