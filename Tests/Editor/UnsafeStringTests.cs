using System;
using System.Text;
using NUnit.Framework;
using UnityEngine;

namespace Resolink.Tests
{
    
    
    public class UnsafeStringTests
    {
        static readonly string StringMemory = "this text is just here to allocate some memory ok ignore it";

        
        static readonly string NewString = "What if it was purple?";
        
        static byte[] NewStringBytes;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            NewStringBytes = Encoding.UTF8.GetBytes(NewString);
        }

        [Test]
        public unsafe void SetStringMemory()
        {
            Debug.Log(StringMemory.GetHashCode());
            
            StringUtil.MemSet(StringMemory, NewString);
            /*
            var byteLength = Encoding.Unicode.GetByteCount(StringMemory);
            var copyByteLength = Encoding.Unicode.GetByteCount(NewString);
            fixed (char* ptr = StringMemory)
            {
                Debug.Log("str pointer : " + (IntPtr)ptr + " , bytes : " + byteLength);
                fixed (char* newPtr = NewString)
                {
                    Debug.Log("copy-from pointer : " + (IntPtr)newPtr + " , bytes : " + copyByteLength);
                    Buffer.MemoryCopy(newPtr, ptr, byteLength, copyByteLength);
                }
            }
            */
            
            Debug.Log("copied-from hash:" + NewString.GetHashCode());
            Debug.Log("copied-to hash:" + StringMemory.GetHashCode());
        }


        static readonly string Simulacra = "simulacra";
        static readonly string Simulation = "simulatin";            // misspelled to make them same # of bytes

        [Test]
        public void EqualLengthMemSet()
        {
            Debug.Log("simulatin before : " + Simulation + " , hash: " +  + Simulation.GetHashCode());
            Debug.Log("simulacra before : " + Simulacra + " , hash: " +  + Simulacra.GetHashCode());

            StringUtil.MemSet(Simulation, Simulacra);
            
            Debug.Log("simulacra after : " + Simulacra + " , hash: " +  + Simulacra.GetHashCode());
            
            Assert.AreEqual(Simulacra, Simulation);
            Assert.AreEqual(Simulacra.GetHashCode(), Simulation.GetHashCode());
        }
    }
}