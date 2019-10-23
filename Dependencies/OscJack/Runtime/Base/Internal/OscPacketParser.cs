// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace OscJack
{
    internal sealed class OscPacketParser
    {
        #region Public Members

        public OscPacketParser(OscMessageDispatcher dispatcher, string appPath = "")
        {
            OutPath = Path.Combine(appPath, LogFilePath);
            Debug.Log("log file: " + OutPath);
            
            LogStream = !File.Exists(OutPath) ? File.Create(OutPath) : File.OpenWrite(OutPath);

            _dispatcher = dispatcher;
        }

        const string LogFilePath = "ResolumeOscInputBytes.txt";

        string OutPath;
        readonly FileStream LogStream;

        static readonly byte[] newLineBytes = Encoding.ASCII.GetBytes("\n");
        
        public void Parse(Byte[] buffer, int length)
        {
            LogStream.Write(buffer, 0, length);
            LogStream.Write(newLineBytes, 0, newLineBytes.Length);
            ScanMessage(buffer, 0, length);
        }

        #endregion

        #region Private Methods

        OscMessageDispatcher _dispatcher;
        OscDataHandle _dataHandle = new OscDataHandle();

        byte[] _copyBuffer = new byte[4098];

        public void Dispose()
        {
            LogStream.Close();
        }

        void ScanMessage(Byte[] buffer, int offset, int length)
        {
            // Where the next element begins if any
            var next = offset + length;

            // OSC address
            var address = OscDataTypes.ReadString(buffer, offset);
            var addressByteLength = OscDataTypes.ReadByteString(buffer, offset, _copyBuffer, 0);
            
            offset += OscDataTypes.Align4(address.Length + 1);

            if (address == "#bundle")
            {
                // We don't use the timestamp data; Just skip it.
                offset += 8;

                // Keep reading until the next element begins.
                while (offset < next)
                {
                    // Get the length of the element.
                    var elementLength = OscDataTypes.ReadInt(buffer, offset);
                    offset += 4;

                    // Scan the bundle element in a recursive fashion.
                    ScanMessage(buffer, offset, elementLength);
                    offset += elementLength;
                }
            }
            else
            {
                // Retrieve the arguments and dispatch the message.
                _dataHandle.Scan(buffer, offset);
                _dispatcher.Dispatch(address, _dataHandle);
                _dispatcher.Dispatch(_copyBuffer, addressByteLength, _dataHandle);
            }
        }

        #endregion
    }
}
