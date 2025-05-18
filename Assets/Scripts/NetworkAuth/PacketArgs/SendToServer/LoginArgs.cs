using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record LoginArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.Login;
        public long SteamId;
    }
}