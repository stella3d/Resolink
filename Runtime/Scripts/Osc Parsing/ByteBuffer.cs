using System;
using System.Text;

namespace Resolink
{
    public class ByteBuffer : IEquatable<ByteAddress>
    {
        public readonly byte[] Buffer;

        public int Capacity => Buffer.Length;
        public int Count { get; protected set; }
        
        public ByteBuffer(int capacity)
        {
            Buffer = new byte[capacity];
        }

        public void Reset()
        {
            Count = 0;
        }

        public void Add(byte b)
        {
            Buffer[Count] = b;
            Count++;
        }

        public bool Equals(ByteAddress other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (Buffer.Length != other.Buffer.Length) return false;
            
            for (var i = 0; i < other.Buffer.Length; i++)
            {
                if (Buffer[i] != other.Buffer[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals((ByteAddress) obj);
        }

        public override int GetHashCode()
        {
            return Buffer.GetHashCode();
        }

        public bool IsAddress(ByteAddress address)
        {
            if (address.Length != Count) return false;
            for (int i = 0; i < address.Buffer.Length; i++)
            {
                if (Buffer[i] != address.Buffer[i])
                    return false;
            }

            return true;
        }
    }
}