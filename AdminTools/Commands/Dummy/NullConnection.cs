using Mirror;
using System;

namespace AdminTools.Commands.Dummy
{
    public sealed class NullConnection : NetworkConnectionToClient
    {
        public override void Send(ArraySegment<byte> segment, int channelId = 0) { }

        public override string address => "localhost";

        public NullConnection(int networkConnectionId) : base(networkConnectionId) { }
    }
}
