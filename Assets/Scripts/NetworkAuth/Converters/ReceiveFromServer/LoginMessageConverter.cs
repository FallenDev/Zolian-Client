using Assets.Scripts.Models;
using Assets.Scripts.NetworkAuth.PacketArgs.ReceiveFromServer;
using Assets.Scripts.NetworkAuth.PacketHandling;
using Assets.Scripts.NetworkAuth.Span;

namespace Assets.Scripts.NetworkAuth.Converters.ReceiveFromServer
{
    public sealed class LoginMessageConverter : PacketConverterBase<LoginMessageArgs>
    {
        protected override LoginMessageArgs Deserialize(ref SpanReader reader)
        {
            var loginMessageType = (PopupMessageType)reader.ReadByte();
            var message = reader.ReadString();
            return new LoginMessageArgs { LoginMessageType = loginMessageType, Message = message };
        }
    }
}