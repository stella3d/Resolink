using System;
using System.Text;

namespace Resolink
{
    public class ByteAddress : IEquatable<ByteAddress>, IEquatable<byte[]>
    {
        public readonly byte[] Buffer;

        public int Length => Buffer.Length;
        
        public ByteAddress(string address)
        {
            Buffer = Encoding.UTF8.GetBytes(address);
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

        public bool Equals(byte[] other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (Buffer.Length != other.Length) return false;

            for (var i = 0; i < other.Length; i++)
            {
                if (Buffer[i] != other[i])
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

        public static bool operator ==(ByteAddress left, ByteAddress right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ByteAddress left, ByteAddress right)
        {
            return !Equals(left, right);
        }
    }
}