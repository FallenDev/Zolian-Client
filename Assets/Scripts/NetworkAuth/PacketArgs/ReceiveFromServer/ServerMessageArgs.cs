using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record ServerMessageArgs : IPacketSerializable
    {
        public PopupMessageType ServerMessageType { get; set; }
        public string Message { get; set; } = null!;
    }
}