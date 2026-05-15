using LiteNetLib;
using LiteNetLib.Utils;
using MessagePack;
using Microsoft.Extensions.Logging;
using MuPegaso.Core.Enums;
using MuPegaso.Core.Interfaces;
using MuPegaso.Network.Packets;

namespace MuPegaso.Network
{
    public class NetworkServer : IDisposable
    {
        private readonly ILogger<NetworkServer> _logger;
        private readonly NetManager _netManager;
        private readonly EventBasedNetListener _listener;
        private readonly CancellationTokenSource _cts = new();
        private Task? _pollTask;

        public event Action<NetPeer>? ClientConnected;
        public event Action<NetPeer, DisconnectInfo>? ClientDisconnected;
        public event Action<NetPeer, PacketType, byte[]>? PacketReceived;
        public int ConnectedCount => _netManager.ConnectedPeersCount;

        public NetworkServer(ILogger<NetworkServer> logger)
        {
            _logger = logger;
            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener) { AutoRecycle = true };
            _listener.ConnectionRequestEvent += req => req.AcceptIfKey("MuPegasoClient");
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
        }

        public void Start(int port = 55901)
        {
            _netManager.Start(port);
            _logger.LogInformation("Servidor iniciado en puerto {Port}", port);
            _pollTask = Task.Run(PollLoop, _cts.Token);
        }

        private async Task PollLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                _netManager.PollEvents();
                await Task.Delay(15, _cts.Token).ConfigureAwait(false);
            }
        }

        private void OnPeerConnected(NetPeer peer)
        {
            _logger.LogInformation("Cliente conectado: {Id} desde {Endpoint}", peer.Id, peer.Address);
            Send(peer, new PongPacket { OriginalTimestamp = 0 });
            ClientConnected?.Invoke(peer);
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo info)
        {
            _logger.LogInformation("Cliente desconectado: {Id} razon: {Reason}", peer.Id, info.Reason);
            ClientDisconnected?.Invoke(peer, info);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod method)
        {
            if (reader.AvailableBytes < 2) return;
            var type = (PacketType)reader.GetUShort();
            var data = reader.GetRemainingBytes();

            if (type == PacketType.Ping)
            {
                long ts = data.Length >= 8 ? BitConverter.ToInt64(data, 0)
                          : DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var writer = new NetDataWriter();
                writer.Put((ushort)PacketType.Pong);
                writer.Put(ts);
                writer.Put(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
                return;
            }
            PacketReceived?.Invoke(peer, type, data);
        }

        public void Send(NetPeer peer, IPacket packet)
        {
            var writer = new NetDataWriter();
            writer.Put((ushort)packet.Type);
            writer.Put(MessagePackSerializer.Serialize(packet.GetType(), packet));
            peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void Broadcast(IPacket packet, NetPeer? exclude = null)
        {
            var writer = new NetDataWriter();
            writer.Put((ushort)packet.Type);
            writer.Put(MessagePackSerializer.Serialize(packet.GetType(), packet));
            var peers = new List<NetPeer>();
            _netManager.GetConnectedPeers(peers);
            foreach (var peer in peers)
                if (peer != exclude) peer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public void Dispose()
        {
            _cts.Cancel();
            _pollTask?.Wait(1000);
            _netManager.Stop();
        }
    }
}
