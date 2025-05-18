using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class VersionConverter : PacketConverterBase<VersionArgs>
    {
        public override void Serialize(ref SpanWriter writer, VersionArgs args) => writer.WriteString(VersionArgs.Version);
    }
}