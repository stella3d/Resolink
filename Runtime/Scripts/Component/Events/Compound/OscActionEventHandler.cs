using System;
using OscJack;

namespace Resolink
{
    /// <summary>
    /// Just like OscEventHandler, but using plain c# events, for better performance at the cost of GUI convenience
    /// </summary>
    /// <typeparam name="T">The data type of the event</typeparam>
    [Serializable]
    public abstract class OscActionEventHandler<T>
    {
        public ResolumeOscShortcut Shortcut;

        public Action<T> Event;

        public OscActionEventHandler(Action<T> action)
        {
            Event = action;
        }

        protected void Register()
        {
            if (Shortcut == null)
                return;
            
            OscRouter.AddCallback(Shortcut.Output.Path, InvokeFromHandle);
        }

        protected void UnRegister()
        {
            if (Shortcut == null)
                return;
            
            OscRouter.RemoveCallback(Shortcut.Output.Path, InvokeFromHandle);
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
        
        // the empty update function is here so the inspector has the disable checkbox
        public void Update() { }
    }
}