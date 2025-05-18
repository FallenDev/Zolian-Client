using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class SoundConverter : PacketConverterBase<SoundArgs>
    {
        protected override SoundArgs Deserialize(ref SpanReader reader)
        {
            var indicatorOrIndex = reader.ReadByte();

            if (indicatorOrIndex == byte.MaxValue)
            {
                var musicIndex = reader.ReadByte();

                return new SoundArgs
                {
                    IsMusic = true,
                    Sound = musicIndex
                };
            }

            return new SoundArgs
            {
                IsMusic = false,
                Sound = indicatorOrIndex
            };
        }
    }
}