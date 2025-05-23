﻿using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.PacketHandling
{
    /// <summary>
    /// A base packet converter that provides shared logic for packet deserialization.
    /// </summary>
    public abstract class PacketConverterBase<T> : IPacketConverter<T> where T : IPacketSerializable
    {
        object IPacketConverter.Deserialize(ref SpanReader reader) => Deserialize(ref reader);
        protected virtual T Deserialize(ref SpanReader reader) => default;
        void IPacketConverter.Serialize(ref SpanWriter writer, object args) => Serialize(ref writer, (T)args);
        public virtual void Serialize(ref SpanWriter writer, T args) { }
    }
}