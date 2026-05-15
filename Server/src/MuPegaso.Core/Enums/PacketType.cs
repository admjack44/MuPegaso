namespace MuPegaso.Core.Enums
{
    public enum PacketType : ushort
    {
        Handshake = 0x0001, HandshakeAck = 0x0002,
        Ping = 0x0003, Pong = 0x0004,
        LoginRequest = 0x0F01, LoginResponse = 0x0F02,
        CharacterListRequest = 0x0F10, CharacterListResponse = 0x0F11,
        CharacterSelectRequest = 0x0F12, CharacterSelectResponse = 0x0F13,
        EnterWorld = 0x1001,
        PlayerMove = 0x1010, PlayerPosition = 0x1011,
        PlayerAttack = 0x1020, PlayerSkill = 0x1021,
        MonsterSpawn = 0x2001, MonsterMove = 0x2002,
        MonsterDie = 0x2003, MonsterAttack = 0x2004,
        ChatMessage = 0x3001,
        ItemDrop = 0x4001, ItemPickup = 0x4002,
        PlayerDamage = 0x5001, PlayerDie = 0x5002,
        PlayerRespawn = 0x5003
    }
}
