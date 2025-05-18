using Assets.Scripts.Extensions;
using Assets.Scripts.NetworkAuth.PacketArgs.SendToServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.SendToServer
{
    public sealed class MovementInputConverter : PacketConverterBase<MovementInputArgs>
    {
        public override void Serialize(ref SpanWriter writer, MovementInputArgs args)
        {
            writer.WriteGuid(args.Serial);
            writer.WriteVector3(args.Position.ToNumerics());
            writer.WriteVector3(args.InputDirection.ToNumerics());
            writer.WriteFloat(args.CameraYaw);
            writer.WriteFloat(args.Speed);
            writer.WriteFloat(args.VerticalVelocity);
        }
    }
}