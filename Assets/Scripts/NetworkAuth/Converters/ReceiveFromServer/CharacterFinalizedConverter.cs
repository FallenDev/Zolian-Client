using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class CharacterFinalizedConverter : PacketConverterBase<CharacterFinalizedArgs>
    {
        protected override CharacterFinalizedArgs Deserialize(ref SpanReader reader)
        {
            var finalized = reader.ReadBoolean();

            return new CharacterFinalizedArgs
            {
                Finalized = finalized
            };
        }
    }
}