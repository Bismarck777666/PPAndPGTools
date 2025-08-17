using System.IO;
using System.Net;
using System.Net.Sockets;
using vtortola.WebSockets.Rfc6455;

namespace vtortola.WebSockets
{
    public sealed class WebSocketFactoryRfc6455 : WebSocketFactory
    {
        public override short Version { get { return 13; } }

        public override WebSocket CreateWebSocket(Stream stream, Socket client, WebSocketListenerOptions options, WebSocketHandshake handshake)
        {
            //Modified by Foresight(2019.09.29)
            if(options.IsParseProxyV1)
                return new WebSocketRfc6455(stream, options, (IPEndPoint)client.LocalEndPoint, handshake.RemoteEndPointFromProxyV1, handshake.Request, handshake.Response, handshake.NegotiatedMessageExtensions);
            else
                return new WebSocketRfc6455(stream, options, (IPEndPoint)client.LocalEndPoint, (IPEndPoint)client.RemoteEndPoint, handshake.Request, handshake.Response, handshake.NegotiatedMessageExtensions);
        }
    }
}
