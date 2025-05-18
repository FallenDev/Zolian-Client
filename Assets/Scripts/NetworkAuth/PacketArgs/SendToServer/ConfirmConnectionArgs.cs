using Assets.Scripts.NetworkAuth.OpCodes;
using Assets.Scripts.NetworkAuth.PacketHandling;

namespace Assets.Scripts.NetworkAuth.PacketArgs.SendToServer
{
    public sealed record ConfirmConnectionArgs : IPacketSerializable
    {
        public static byte OpCode => (byte)ClientOpCode.ClientRedirected;
        public static string Message => "Redirect Successful";
    }
}