using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class EnterGameConverter : PacketConverterBase<EnterGameArgs>
    {
        public override void Serialize(ref SpanWriter writer, EnterGameArgs args)
        {
            writer.WriteGuid(args.Serial);
            writer.WriteInt64(args.SteamId);
            writer.WriteString(args.UserName);
        }
    }
}