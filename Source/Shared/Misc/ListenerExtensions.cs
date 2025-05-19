namespace Shared 
{
    public static class ListenerExtensions 
    {
        public static void EnqueueModdedPacket(this ListenerBase listener, ModdedPacketTypes type, object data)
        {
            PacketHeader header = (PacketHeader)type;
            listener.EnqueuePacket(header, data);
        }
    }
}