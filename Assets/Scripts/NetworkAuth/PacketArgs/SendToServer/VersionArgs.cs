using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;
using UnityEngine;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record VersionArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.Version;
        public static string Version => Application.version;
    }
}