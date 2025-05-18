using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record ConnectionInfoArgs : IPacketSerializable
    {
        public ushort PortNumber { get; set; }
    }
}