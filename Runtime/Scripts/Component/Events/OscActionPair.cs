using System;
using OscJack;

namespace Resolink
{
    public class OscActionPair
    {
        /// <summary>
        /// This callback runs immediately on the data receiving thread
        /// </summary>
        public readonly Action<OscDataHandle> ValueRead;
        
        /// <summary>
        /// This is the event that is wired in the UI, queued on the main thread
        /// </summary>
        public readonly Action UserCallback;

        public OscActionPair(Action<OscDataHandle> valueRead, Action userCallback)
        {
            ValueRead = valueRead;
            UserCallback = userCallback;
        }
    }
}