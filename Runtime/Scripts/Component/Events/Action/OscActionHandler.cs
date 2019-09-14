using System;
using OscJack;

namespace Resolink
{
    /// <summary>
    /// Like OscEventHandler, but using plain c# events & classes, for better performance at the cost of GUI convenience
    /// </summary>
    /// <typeparam name="T">The data type of the event</typeparam>
    [Serializable]
    public abstract class OscActionHandler<T>
    {
        public ResolumeOscShortcut Shortcut;

        public Action<T> Event;

        protected OscActionHandler(Action<T> action)
        {
            Event = action;
        }

        /// <summary>
        /// Extract a typed value from a data handle. 
        /// </summary>
        /// <param name="dataHandle">The handle to extract from</param>
        /// <returns>The message value</returns>
        protected abstract T GetMessageValue(OscDataHandle dataHandle);

        public void InvokeFromHandle(OscDataHandle dataHandle)
        {
            Event.Invoke(GetMessageValue(dataHandle));
        }
    }
}