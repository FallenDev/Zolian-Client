using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record EnterWorldServerArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.EnterWorld;
        public ushort Port;
    }
}