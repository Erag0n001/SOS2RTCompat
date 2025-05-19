using GameServer.TCP;
using Shared;
using Shared.SOS2RTCompat;

namespace GameServer.SOS2RTCompat
{
    public static class SOS2GlobalDataManager
    {
        public static void SendServerSpaceSettlements(ServerClient client)
        {
            List<SpaceSettlementData> tempList = new List<SpaceSettlementData>();
            SpaceSettlementFile[] settlements = SpaceSettlementManager.GetAllSettlements();
            foreach (SpaceSettlementFile settlement in settlements)
            {
                SpaceSettlementData data = new SpaceSettlementData();

                if (settlement.UID == client.UserFile.Uid) continue;
                else
                {
                    data._settlementFile = settlement;
                    data._stepMode = CommonEnumerators.SettlementStepMode.Add;

                    tempList.Add(data);
                }
            }
            GlobalData global = new GlobalData() { _spaceSettlements = tempList.ToArray()};
            client.Listener.EnqueueModdedPacket(ModdedPacketTypes.GlobalSpaceSettlements, global);
        }
    }
}
