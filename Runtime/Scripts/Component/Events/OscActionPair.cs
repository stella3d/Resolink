using System;
using OscJack;

namespace Resolink
{
    public class OscActionPair
    {
        /// <summary>
        /// This callback runs immediately on the data receiving thread
        /// </summary>
        public readonly Func<OscDataHandle, bool> ValueRead;
        
        /// <summary>
        /// This is the event that is wired in the UI, queued on the main thread
        /// </summary>
        public readonly Action UserCallback;

        public OscActionPair(Func<OscDataHandle, bool> valueRead, Action userCallback)
        {
            ValueRead = valueRead;
            UserCallback = userCallback;
        }
    }
}