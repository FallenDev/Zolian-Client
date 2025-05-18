using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record AcceptConnectionArgs : IPacketSerializable
    {
        public string Message { get; set; }
    }
}