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
            NewStringBytes = Encoding.ASCII.GetBytes(NewString);
        }

        [Test]
        public unsafe void SetStringMemory()
        {
            Debug.Log(StringMemory.GetHashCode());
            
            StringUtil.MemSet(StringMemory, NewString);

            Debug.Log("copied-from hash:" + NewString.GetHashCode());
            Debug.Log("copied-to hash:" + StringMemory.GetHashCode());
        }


        static readonly string OscAddress = "/composition/layers/1/effects";
        static readonly string OscAddressToMutate = "/abcdositxyzl/lazrs/5a/ffnice";
        
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

        [Test]
        public void AsciiToUnicodeBytes()
        {
            var unicode = Encoding.Unicode.GetBytes(OscAddress);
            var ascii = Encoding.ASCII.GetBytes(OscAddress);

            var converted = new byte[unicode.Length];
            
            StringUtil.AsciiToUnicode(ascii, 0, ascii.Length, converted);
            
            for (int i = 0; i < unicode.Length; i++)
            {
                Assert.AreEqual(unicode[i], converted[i]);
            }
            
            //Debug.Log($"ascii {asciiSimulacra.Length}, unicode {unicodeSimulacra.Length}");
        }

        [Test]
        public unsafe void AsciiOverwriteUnchecked()
        {
            //var unicode = Encoding.Unicode.GetBytes(OscAddress);
            var ascii = Encoding.ASCII.GetBytes(OscAddress);
            
            Assert.False(OscAddress == OscAddressToMutate);

            StringUtil.AsciiOverwriteUnchecked(ascii, 0, ascii.Length, OscAddressToMutate);

            Debug.Log("after direct mutate: " + OscAddressToMutate);
            
            // hashing works ?
            Assert.AreEqual(OscAddress.GetHashCode(), OscAddressToMutate.GetHashCode());

            // make sure that sorting the strings still works the same
            Assert.Zero(string.Compare(OscAddress, OscAddressToMutate, StringComparison.Ordinal));

            // equality operators still work ?
            Assert.True(OscAddress == OscAddressToMutate);

            // all bytes are the same ?
            Assert.AreEqual(OscAddressToMutate.Length, OscAddress.Length); 
            for (int i = 0; i < OscAddressToMutate.Length; i++)
                Assert.AreEqual(OscAddress[i], OscAddressToMutate[i]);
            
            // pointers are NOT the same ?
            fixed (char* srcPtr = OscAddress)
            {
                fixed (char* destPtr = OscAddressToMutate)
                {
                    Debug.Log("src ptr: " + (IntPtr)srcPtr + " , dest ptr: " + (IntPtr)destPtr);
                    Assert.False(srcPtr == destPtr);
                }
            }
        }

        [Test]
        public void MemSetToConst_DoesNothing()
        {
            const string constStr = "constant";
            const string copyFrom = "copyfrom";
            
            Assert.True(copyFrom != constStr);
            
            StringUtil.MemSet(copyFrom, constStr);
                        
            Assert.True(constStr.StartsWith("const"));
            Assert.True(copyFrom != constStr);
        }
        
        [Test]
        public void MemSetDebug()
        {
            var str = "constant2";
            var copyStr = "copyfrom2";
            Assert.True(copyStr != str);
            
            StringUtil.MemSet(copyStr, str);
                        
            Assert.True(copyStr.StartsWith("copy"));
        }

    }
}