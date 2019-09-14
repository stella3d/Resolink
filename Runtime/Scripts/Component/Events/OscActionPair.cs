using System;
using OscJack;

namespace Resolink
{
    public class OscActionPair
    {
        public Action<OscDataHandle> ValueRead;
        public Action UserCallback;

        public OscActionPair(Action<OscDataHandle> valueRead, Action userCallback)
        {
            ValueRead = valueRead;
            UserCallback = userCallback;
        }
    }
}