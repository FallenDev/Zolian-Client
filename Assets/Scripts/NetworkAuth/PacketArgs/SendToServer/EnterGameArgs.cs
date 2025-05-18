using System;
using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record EnterGameArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.EnterGame;
        public Guid Serial;
        public long SteamId;
        public string UserName;
    }
}