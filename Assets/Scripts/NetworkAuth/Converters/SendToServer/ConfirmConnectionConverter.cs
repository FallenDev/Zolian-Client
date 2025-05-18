using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class ConfirmConnectionConverter : PacketConverterBase<ConfirmConnectionArgs>
    {
        public override void Serialize(ref SpanWriter writer, ConfirmConnectionArgs args) => writer.WriteString(ConfirmConnectionArgs.Message);
    }
}