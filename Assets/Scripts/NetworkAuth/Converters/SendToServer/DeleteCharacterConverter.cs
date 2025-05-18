using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class DeleteCharacterConverter : PacketConverterBase<DeleteCharacterArgs>
    {
        public override void Serialize(ref SpanWriter writer, DeleteCharacterArgs args)
        {
            writer.WriteGuid(args.Serial);
            writer.WriteInt64(args.SteamId);
            writer.WriteString(args.Name);
        }
    }
}