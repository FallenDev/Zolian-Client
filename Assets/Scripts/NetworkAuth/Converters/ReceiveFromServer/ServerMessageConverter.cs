using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class ServerMessageConverter : PacketConverterBase<ServerMessageArgs>
    {
        protected override ServerMessageArgs Deserialize(ref SpanReader reader)
        {
            var messageType = reader.ReadByte();
            var message = reader.ReadString();

            return new ServerMessageArgs
            {
                ServerMessageType = (PopupMessageType)messageType,
                Message = message
            };
        }
    }
}