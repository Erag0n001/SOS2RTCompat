using Shared.SOS2RTCompat;

namespace GameServer.SOS2RTCompat
{
    public static class IDManager
    {
        private static int currentID = -1;
        public static int GetNextID()
        {
            return ++currentID;
        }

        public static void SetCurrentIDOnLoad()
        {
            SpaceSettlementFile[] settlements = SpaceSettlementManager.GetAllSettlements();
            int id = 0;
            foreach (SpaceSettlementFile settlement in settlements)
            {
                if (settlement.ID > id)
                    id = settlement.ID;
            }
            currentID = id;
        }
    }
}
