using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer
{
    public sealed record LoginMessageArgs : IPacketSerializable
    {
        public PopupMessageType LoginMessageType { get; set; }
        public string Message { get; set; }
    }
}