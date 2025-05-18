using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record CharacterFinalizedArgs : IPacketSerializable
    {
        public bool Finalized { get; set; }
    }
}