using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class AcceptConnectionConverter : PacketConverterBase<AcceptConnectionArgs>
    {
        protected override AcceptConnectionArgs Deserialize(ref SpanReader reader)
        {
            var message = reader.ReadString();
            return new AcceptConnectionArgs { Message = message };
        }
    }
}
