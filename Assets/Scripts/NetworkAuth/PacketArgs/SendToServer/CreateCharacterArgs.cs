using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record CreateCharacterArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.CreateCharacter;
        public long SteamId;
        public string Name;
        public BaseClass Class;
        public Race Race;
        public Sex Sex;
        public short Hair;
        public short HairColor;
        public short HairHighlightColor;
        public short SkinColor;
        public short EyeColor;
        public short Beard;
        public short Mustache;
        public short Bangs;
    }
}