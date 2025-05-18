using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class ConnectionInfoConverter : PacketConverterBase<ConnectionInfoArgs>
    {
        protected override ConnectionInfoArgs Deserialize(ref SpanReader reader)
        {
            var port = reader.ReadUInt16();
            return new ConnectionInfoArgs { PortNumber = port };
        }
    }
}
