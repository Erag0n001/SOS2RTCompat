using System;

namespace Shared 
{
    public class HandlesModdedPacket : Attribute 
    {
        public readonly ModdedPacketTypes header;
        public HandlesModdedPacket(ModdedPacketTypes header) 
        {
            this.header = header;
        }
    }
}