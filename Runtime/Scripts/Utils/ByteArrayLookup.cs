using System.Collections.Generic;
using System.Text;

namespace Resolink
{
    public class ByteArrayLookup <T>
    {
        public Dictionary<int, List<KeyValuePair<byte[], T>>> BucketsByLength = 
            new Dictionary<int, List<KeyValuePair<byte[], T>>>();

        public void Add(string key, T value)
        {
            var bytes = Encoding.UTF8.GetBytes(key);
            
            if (!BucketsByLength.TryGetValue(bytes.Length, out var bucket))
                bucket = new List<KeyValuePair<byte[], T>>();
            
            bucket.Add(new KeyValuePair<byte[], T>(bytes, value));
        }

        public bool TryGetValue(byte[] buffer, int start, int length, out T value)
        {
            if (!BucketsByLength.TryGetValue(length, out var bucket))
            {
                value = default;
                return false;
            }

            var end = start + length;
            foreach (var kvp in bucket)
            {
                var found = true;
                var byteString = kvp.Key;
                for (var i = start; i < end; i++)
                {
                    var searchInputByte = buffer[i];
                    var storedStringByte = byteString[i];
                    if (searchInputByte != storedStringByte)
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    value = kvp.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }
    }
}