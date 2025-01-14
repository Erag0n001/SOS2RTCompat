namespace Shared.SOS2RTCompat
{
    public class SpaceSettlementFile : SettlementFile
    {
        public int ID; //This should NEVER be -1
        public float Theta;
        public float Radius;
        public float Phi;
        public SpaceSettlementFile(int id) 
        {
            ID = id;
        }
    }
}