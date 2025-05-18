using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record SoundArgs : IPacketSerializable
    {
        public bool IsMusic { get; set; }
        public byte Sound { get; set; }
    }
}