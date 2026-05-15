using MessagePack;
using MuPegaso.Core.Enums;
using MuPegaso.Core.Interfaces;

namespace MuPegaso.Network.Packets
{
    public abstract class BasePacket : IPacket { [IgnoreMember] public abstract PacketType Type { get; } }

    [MessagePackObject] public class PingPacket : BasePacket {
        public override PacketType Type => PacketType.Ping;
        [Key(0)] public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    [MessagePackObject] public class PongPacket : BasePacket {
        public override PacketType Type => PacketType.Pong;
        [Key(0)] public long OriginalTimestamp { get; set; }
        [Key(1)] public long ServerTimestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    [MessagePackObject] public class LoginRequestPacket : BasePacket {
        public override PacketType Type => PacketType.LoginRequest;
        [Key(0)] public string Username { get; set; } = "";
        [Key(1)] public string Password { get; set; } = "";
    }
    [MessagePackObject] public class LoginResponsePacket : BasePacket {
        public override PacketType Type => PacketType.LoginResponse;
        [Key(0)] public bool Success { get; set; }
        [Key(1)] public string Message { get; set; } = "";
        [Key(2)] public string Token { get; set; } = "";
    }
    [MessagePackObject] public class PlayerMovePacket : BasePacket {
        public override PacketType Type => PacketType.PlayerMove;
        [Key(0)] public int PlayerId { get; set; }
        [Key(1)] public float X { get; set; }
        [Key(2)] public float Y { get; set; }
        [Key(3)] public float Z { get; set; }
        [Key(4)] public float Rotation { get; set; }
    }
    [MessagePackObject] public class ChatMessagePacket : BasePacket {
        public override PacketType Type => PacketType.ChatMessage;
        [Key(0)] public string Sender { get; set; } = "";
        [Key(1)] public string Message { get; set; } = "";
        [Key(2)] public byte Channel { get; set; }
    }
}
