// derived from OSC Jack - https://github.com/keijiro/OscJack
using System;
using System.Collections.Generic;
using OscJack;

namespace Resolink
{
    internal sealed class CustomOscPacketParser
    {
        public CustomOscPacketParser(CustomOscMessageDispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
        }

        public void Parse(Byte[] buffer, int length)
        {
            ScanMessage(buffer, 0, length);
        }

        CustomOscMessageDispatcher m_Dispatcher;
        OscDataHandle _dataHandle = new OscDataHandle();

        ByteBuffer m_AddressBuffer = new ByteBuffer(512);
        
        void ScanMessage(Byte[] buffer, int offset, int length)
        {
            // Where the next element begins if any
            var next = offset + length;

            // OSC address
            var address = OscDataTypes.ReadString(buffer, offset);
            offset += OscDataTypes.Align4(address.Length + 1);
            
            // NEW APPROACH
            var addressLength = ReadByteAddress(buffer, offset, m_AddressBuffer);
            offset += OscDataTypes.Align4(addressLength + 1);

            // Retrieve the arguments and dispatch the message.
            _dataHandle.Scan(buffer, offset);
            m_Dispatcher.Dispatch(m_AddressBuffer, _dataHandle);
        }
        
        public static int ReadByteAddress(Byte[] buffer, int offset, ByteBuffer addressBuffer)
        {
            addressBuffer.Reset();
            byte current;
            do
            {
                current = buffer[offset + addressBuffer.Count];
                addressBuffer.Add(current);
            } 
            while (current != 0);

            return addressBuffer.Count;
        }
    }
}
