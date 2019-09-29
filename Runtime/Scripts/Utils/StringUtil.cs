using System;
using System.Text;

namespace Resolink
{
    public static class StringUtil
    {
        public static unsafe void MemSet(string source, string destination, int srcByteOffset = 0, int destByteOffset = 0)
        {
            var byteLength = Encoding.Unicode.GetByteCount(source);
            var copyByteLength = Encoding.Unicode.GetByteCount(destination);
            
            fixed (char* srcPtr = source)
            {
                fixed (char* destPtr = destination)
                {
                    Buffer.MemoryCopy(srcPtr + srcByteOffset, destPtr + destByteOffset, byteLength, copyByteLength);
                }
            }
        }
        
        public static unsafe void MemSet(byte[] source, int sourceLength, string destination, int destLength, 
            int srcOffset, int destOffset)
        {
            fixed (byte* srcPtr = source)
            {
                //Debug.Log("str pointer : " + (IntPtr)ptr + " , bytes : " + byteLength);
                fixed (char* destPtr = destination)
                {
                    //Debug.Log("copy-from pointer : " + (IntPtr)newPtr + " , bytes : " + copyByteLength);
                    Buffer.MemoryCopy(srcPtr + srcOffset, destPtr + destOffset, destLength, sourceLength);
                }
            }
        }
        
        public static unsafe void MemSet(byte[] source, int sourceLength, string destination,
            int srcOffset = 0, int destOffset = 0)
        {
            var destLength = Encoding.Unicode.GetByteCount(destination);
            MemSet(source, sourceLength, destination, destLength, srcOffset, destOffset);
        }
        
        public static unsafe void MemSetEqualLength(byte[] source, int sourceLength, string destination,
            int srcOffset = 0)
        {
            MemSet(source, sourceLength, destination, sourceLength, srcOffset, 0);
        }
    }
}