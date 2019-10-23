using System;
using System.Text;
using UnityEngine;

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

        public static void AsciiToUnicode(byte[] ascii, int asciiStart, int asciiLength, 
            byte[] unicode, int unicodeStart = 0)
        {
            var end = asciiStart + asciiLength;
            for (var i = asciiStart; i < end; i++)
            {
                var a = ascii[i];
                var uStart = unicodeStart + (i * 2);
                unicode[uStart] = a;
                //this method counts on the fact that all ASCII chars are encoded in the first byte
                unicode[uStart + 1] = 0;
            }
        }
        
        /*
        public static unsafe void AsciiStringOverwrite(byte[] ascii, int asciiStart, int asciiLength, 
            byte[] unicode, int unicodeStart, string toMutate)
        {
            var end = asciiStart + asciiLength;
            // could probably do without this first copy and just directly set the string memory ?
            for (var i = asciiStart; i < end; i++)
            {
                var a = ascii[i];
                var uStart = unicodeStart + (i * 2);
                unicode[uStart] = a;
                //this method counts on the fact that all ASCII chars are encoded in the first byte
                unicode[uStart + 1] = 0;
            }
            
            MemSet(unicode, asciiLength * 2, toMutate, unicodeStart, 0);
        }
        */
        
        /// <summary>
        /// Overwrite an existing string with ascii-encoded bytes.
        /// This doesn't do any safety checks, so be sure about it!
        /// </summary>
        /// <param name="ascii">The byte array to copy from</param>
        /// <param name="start">The index in the byte array to start at</param>
        /// <param name="length">The number of ascii characters to copy</param>
        /// <param name="toMutate">The string to overwrite</param>
        public static unsafe void AsciiOverwriteUnchecked(byte[] ascii, int start, int length, 
            string toMutate)
        {
            var end = start + length;
            fixed (char* ptr = toMutate)
            {
                for (var i = start; i < end; i++)
                    ptr[i - start] = (char)ascii[i];
            }
        }
        
        /// <summary>
        /// Overwrite an existing string with ascii-encoded bytes.
        /// This does bounds checks, and should be safe to call even if parameters are wrong.
        /// </summary>
        /// <param name="ascii">The byte array to copy from</param>
        /// <param name="start">The index in the byte array to start at</param>
        /// <param name="length">The number of ascii characters to copy</param>
        /// <param name="toMutate">The string to overwrite</param>
        public static unsafe void AsciiOverwrite(byte[] ascii, int start, int length, 
            string toMutate)
        {
            var end = start + length;
            if(end > ascii.Length)
                throw new ArgumentOutOfRangeException(nameof(length), 
                    $"start + length = {end} is beyond array length {ascii.Length}");
            
            if(toMutate.Length != length)
                throw new ArgumentOutOfRangeException(nameof(toMutate), 
                    $"string to mutate must have the same number of characters as the input byte length {length}");
            
            fixed (char* ptr = toMutate)
            {
                for (var i = start; i < end; i++)
                    ptr[i - start] = (char)ascii[i];
            }
        }
    }
}