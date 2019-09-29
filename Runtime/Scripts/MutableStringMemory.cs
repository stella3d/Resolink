using System.Collections.Generic;
using System.Text;
using UnityEditor.UIElements;

namespace Resolink
{
    public class MutableStringMemory
    {
        public Encoding Encoding = Encoding.UTF8;
        
        internal Dictionary<int, string> ByteLengthToMemory = new Dictionary<int, string>();




        public void Assign(byte[] buffer, int start, int length)
        {
            if (ByteLengthToMemory.TryGetValue(length, out var memory))
                StringUtil.MemSetEqualLength(buffer, length, memory);
            else
                memory = Encoding.GetString(buffer, start, length);

            ByteLengthToMemory[length] = memory;
        }
    }
}