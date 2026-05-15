using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;

namespace MuPegaso.Client.Network
{
    public class NetworkClient : MonoBehaviour, INetEventListener
    {
        public static NetworkClient Instance { get; private set; }

        [Header("Config")]
        [SerializeField] private string host = "127.0.0.1";
        [SerializeField] private int port = 55901;

        private NetManager _netManager;
        private NetPeer _server;
        private Thread _pollThread;
        private bool _running;

        public bool IsConnected => _server?.ConnectionState == ConnectionState.Connected;
        public event Action OnConnected;
        public event Action<string> OnDisconnected;
        public event Action<float> OnPongReceived;

        void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _netManager = new NetManager(this) { AutoRecycle = true };
            _netManager.Start();
            _netManager.Connect(host, port, "MuPegasoClient");
            Debug.Log($"[Network] Conectando a {host}:{port}...");
            _running = true;
            _pollThread = new Thread(() => {
                while (_running) { _netManager.PollEvents(); Thread.Sleep(15); }
            }) { IsBackground = true };
            _pollThread.Start();
        }

        public void SendPing()
        {
            if (!IsConnected) return;
            var w = new NetDataWriter();
            w.Put((ushort)0x0003);
            w.Put(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            _server.Send(w, DeliveryMethod.ReliableOrdered);
        }

        public void OnPeerConnected(NetPeer peer)
        {
            _server = peer;
            Debug.Log("[Network] CONECTADO");
            UnityMainThread.Post(() => OnConnected?.Invoke());
        }

        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo info)
        {
            Debug.Log($"[Network] DESCONECTADO: {info.Reason}");
            UnityMainThread.Post(() => OnDisconnected?.Invoke(info.Reason.ToString()));
        }

        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte ch, DeliveryMethod method)
        {
            if (reader.AvailableBytes < 2) return;
            ushort type = reader.GetUShort();
            if (type == 0x0004)
            {
                if (reader.AvailableBytes >= 16)
                {
                    long orig = reader.GetLong();
                    long srv  = reader.GetLong();
                    float ms  = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - orig;
                    Debug.Log($"[Network] PONG {ms:F0}ms");
                    UnityMainThread.Post(() => OnPongReceived?.Invoke(ms));
                }
            }
        }

        public void OnConnectionRequest(ConnectionRequest r) => r.AcceptIfKey("MuPegasoClient");
        public void OnNetworkError(IPEndPoint ep, SocketError err) => Debug.LogError($"[Network] {err}");
        public void OnNetworkReceiveUnconnected(IPEndPoint ep, NetPacketReader r, UnconnectedMessageType t) {}
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {}

        void OnDestroy() { _running = false; _netManager?.Stop(); }
    }
}
