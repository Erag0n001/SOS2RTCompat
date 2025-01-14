namespace Shared.SOS2RTCompat
{
    public class MovementData
    {
        public float _phi;
        public float _theta;
        public float _radius;
        public int _tile;
        public int shipID;
        public MovementData(int id) 
        {
            shipID = id; //If we set the id to -1, it means we assume the client sending is the owner of the ship and we'll use UID
        }
    }
}