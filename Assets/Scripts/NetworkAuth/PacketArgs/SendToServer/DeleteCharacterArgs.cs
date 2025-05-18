using System;
using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record DeleteCharacterArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.DeleteCharacter;
        public Guid Serial;
        public long SteamId;
        public string Name;
    }
}