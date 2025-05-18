using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class CreateCharacterConverter : PacketConverterBase<CreateCharacterArgs>
    {
        public override void Serialize(ref SpanWriter writer, CreateCharacterArgs args)
        {
            writer.WriteInt64(args.SteamId);
            writer.WriteString(args.Name);
            writer.WriteByte((byte)args.Class);
            writer.WriteByte((byte)args.Race);
            writer.WriteByte((byte)args.Sex);
            writer.WriteInt16(args.Hair);
            writer.WriteInt16(args.HairColor);
            writer.WriteInt16(args.HairHighlightColor);
            writer.WriteInt16(args.SkinColor);
            writer.WriteInt16(args.EyeColor);
            writer.WriteInt16(args.Beard);
            writer.WriteInt16(args.Mustache);
            writer.WriteInt16(args.Bangs);
        }
    }
}