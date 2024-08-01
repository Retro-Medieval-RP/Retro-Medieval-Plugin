using System;
using System.Net;
using SDG.NetTransport;
using SDG.Unturned;

namespace AiBots.Bot;

internal class BotTransportConnection
{
    private readonly string m_IP;
    private readonly ushort m_Port;
    private readonly IPAddress m_Address;

    public BotTransportConnection()
    {
        var random = new Random();
        m_IP = $"{(object)random.Next(1, 256)}.{(object)random.Next(256)}.{(object)random.Next(256)}.{(object)random.Next(256)}";
        m_Port = (ushort) random.Next(1, 65536);
        m_Address = IPAddress.Parse(m_IP);
    }

    public void CloseConnection()
    {
    }

    public bool Equals(ITransportConnection other) => false;

    public bool TryGetIPv4Address(out uint address)
    {
        address = Parser.getUInt32FromIP(m_IP);
        return true;
    }

    public bool TryGetPort(out ushort port)
    {
        port = m_Port;
        return true;
    }

    public IPAddress GetAddress() => m_Address;

    public string GetAddressString(bool withPort) => m_IP + (withPort ? ":" + m_Port.ToString() : string.Empty);

    public void Send(byte[] buffer, long size, ENetReliability sendType)
    {
    }
}