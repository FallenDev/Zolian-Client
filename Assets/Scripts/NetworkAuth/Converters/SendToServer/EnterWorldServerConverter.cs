using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class EnterWorldServerConverter : PacketConverterBase<EnterWorldServerArgs>
    {
        public override void Serialize(ref SpanWriter writer, EnterWorldServerArgs args)
        {
            writer.WriteUInt16(args.Port);
        }
    }
}