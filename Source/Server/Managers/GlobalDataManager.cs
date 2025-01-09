using GameServer.TCP;
using Shared;
using Shared.SOS2RTCompat;

namespace GameServer.SOS2RTCompat
{
    [RTManager]
    public static class SOS2GlobalDataManager
    {
        public static void SendServerSpaceSettlements(ServerClient client)
        {
            List<SpaceSettlementData> tempList = new List<SpaceSettlementData>();
            SpaceSettlementFile[] settlements = SpaceSettlementManager.GetAllSettlements();
            foreach (SpaceSettlementFile settlement in settlements)
            {
                SpaceSettlementData data = new SpaceSettlementData();

                if (settlement.UID == client.userFile.Uid) continue;
                else
                {
                    data._settlementFile = settlement;
                    data._stepMode = CommonEnumerators.SettlementStepMode.Add;

                    tempList.Add(data);
                }
            }
            GlobalData global = new GlobalData() { _spaceSettlements = tempList.ToArray()};
            Packet packet = Packet.CreatePacketFromObject(nameof(SOS2GlobalDataManager), global);
            client.listener.EnqueuePacket(packet);
        }
    }
}
