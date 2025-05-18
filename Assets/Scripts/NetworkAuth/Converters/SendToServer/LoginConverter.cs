using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class LoginConverter : PacketConverterBase<LoginArgs>
    {
        public override void Serialize(ref SpanWriter writer, LoginArgs args)
        {
            writer.WriteInt64(args.SteamId);
        }
    }
}