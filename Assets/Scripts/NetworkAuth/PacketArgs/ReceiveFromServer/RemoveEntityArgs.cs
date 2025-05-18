using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record RemoveEntityArgs : IPacketSerializable
    {
        public uint SourceId { get; set; }
    }
}