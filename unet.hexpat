#pragma endian big

struct PacketBaseHeader {
    u16 connectionId;
};

enum SystemRequestType : u8 {
    ConnectRequest = 0x01,
    Disconnect = 0x03,
    Ping = 0x04
};

struct SystemPacket {
    SystemRequestType requestType;
    u16 packetId;
    u16 sessionId;
    u16 localConnectionId;
    u16 remoteConnectionId;
};

struct ConnectPacket {
    u32 libVersion;
    u32 crc;
};

enum NetworkError : u8 {
    Ok,
    WrongHost,
    WrongConnection,
    WrongChannel,
    NoResources,
    BadMessage,
    Timeout,
    MessageToLong,
    WrongOperation,
    VersionMismatch,
    CRCMismatch,
    DNSFailure
};

struct DisconnectPacket {
    u32 libVersion;
    NetworkError reason;
};

struct PingPacket {
    u32 sentPingTime;
    u32 ackPingTime;
    u32 localTimeCorrection;
    u8 intDropRate;
    u8 extDropRate;
    u16 remoteSessionId;
};

struct Packet {
    PacketBaseHeader baseHeader [[name("Base Header")]];
    
    if (this.baseHeader.connectionId == 0) {
        SystemPacket system [[name("System")]];
        
        if (this.system.requestType == SystemRequestType::ConnectRequest) {
            ConnectPacket connect [[name("Connect Packet")]];
        } else if (this.system.requestType == SystemRequestType::Disconnect) {
            DisconnectPacket disconnect [[name("Disconnect Packet")]];
        } else if (this.system.requestType == SystemRequestType::Ping) {
            PingPacket ping [[name("Ping Packet")]];
        }
    } else {
    }
};

Packet packet @ 0x00;