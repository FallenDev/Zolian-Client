using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class RemoveEntityConverter : PacketConverterBase<RemoveEntityArgs>
    {
        protected override RemoveEntityArgs Deserialize(ref SpanReader reader)
        {
            var sourceId = reader.ReadUInt32();

            return new RemoveEntityArgs
            {
                SourceId = sourceId
            };
        }
    }
}