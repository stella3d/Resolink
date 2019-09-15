// derived from OSC Jack - https://github.com/keijiro/OscJack
using System;
using System.Collections.Generic;
using OscJack;

namespace Resolink
{
    public sealed class CustomOscMessageDispatcher
    {
        public delegate void MessageCallback(ByteBuffer addressBuffer, OscDataHandle data);
        
        public readonly Dictionary<string, OscActionPair> AddressHandlers = 
            new Dictionary<string, OscActionPair>(8);


        public ByteAddress[] m_ByteAddresses = new ByteAddress[8];
        public OscActionPair[] m_AddressHandlers = new OscActionPair[8];
        
        MessageCallback m_MonitorCallback;

        public void SetMonitorCallback(MessageCallback callback)
        {
            m_MonitorCallback = callback;
        }

        public void RemoveMonitorCallback()
        {
            m_MonitorCallback = null;
        }

        public void MonitorCallback(ByteBuffer addressBuffer, OscDataHandle data)
        {
            for (int i = 0; i < m_ByteAddresses.Length; i++)
            {
                var address = m_ByteAddresses[i];
                if (addressBuffer.IsAddress(address))
                {
                    var handlerPair = m_AddressHandlers[i];
                    handlerPair.ValueRead(data);
                }
            }
        }

        public void Dispatch(ByteBuffer addressBuffer, OscDataHandle data)
        {
            m_MonitorCallback(addressBuffer, data);
        }
    }
}
