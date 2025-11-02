#pragma endian big
import std.mem;

u8 channelType in;

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

struct NetPacketHeader {
    u16 packetId;
    u16 sessionId;
};

struct PacketAcks32 {
    u16 ackMessageId;
    u32 acks[1];
};

enum QosType : u8 {
    Unreliable,
    UnreliableFragmented,
    UnreliableSequenced,
    Reliable,
    ReliableFragmented,
    ReliableSequenced,
    StateUpdate,
    ReliableStateUpdate,
    AllCostDelivery
};

struct NetMessageReliableHeader {
    u16 messageId;
};

struct NetMessageOrderedHeader {
    u8 orderedMessageId;
};

struct NetMessageFragmentedHeader {
    u8 fragmentedMessageId;
    u8 fragmentIndex;
    u8 fragmentAmount;
};

struct UserPacket {
    u8 channelId;
    u16 length;
    
    u8 headerLength = 3 [[hidden]];
    if (channelType == QosType::Reliable || channelType == QosType::ReliableFragmented || channelType == QosType::ReliableSequenced || channelType == QosType::ReliableStateUpdate || channelType == QosType::AllCostDelivery) {
        NetMessageReliableHeader reliableHeader;
        headerLength += 2;
    }
    
    if (channelType == QosType::UnreliableSequenced || channelType == QosType::ReliableSequenced) {
        NetMessageOrderedHeader orderedHeader;
        headerLength += 1;
    }
    
    if (channelType == QosType::UnreliableFragmented || channelType == QosType::ReliableFragmented) {
        NetMessageFragmentedHeader fragmentedHeader;
        headerLength += 3;
    }
    
    u8 payload[this.length - headerLength];
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
        NetPacketHeader netPacket [[name("User Packet Header")]];
        PacketAcks32 packetAcks [[name("Packet Acks")]];
        UserPacket userPackets[while(!std::mem::eof())] [[name("User Packets")]];
    }
};

Packet packet @ 0x00;