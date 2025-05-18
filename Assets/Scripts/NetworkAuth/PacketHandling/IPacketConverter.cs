using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.PacketHandling
{
    /// <summary>
    /// Defines a pattern to deserialize a span of bytes into an object.
    /// </summary>
    public interface IPacketConverter
    {
        object Deserialize(ref SpanReader reader);
        void Serialize(ref SpanWriter writer, object args);
    }

    public interface IPacketConverter<in T> : IPacketConverter where T : IPacketSerializable { }
}