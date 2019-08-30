using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Resolunity
{
    public static class ShortcutExtensions
    {
        public static bool IsTimeEvent(this ResolumeOscShortcut shortcut)
        {
            const string tempoController = "/composition/tempocontroller/";
            const string tempoInputPath = tempoController + "tempo";
            const string pauseInputPath = tempoController + "pause";

            var inputPath = shortcut.Input.Path;
            var isTimeEvent = inputPath == tempoInputPath || inputPath == pauseInputPath;
            if (isTimeEvent)
            {
                Debug.Log("time event: " + shortcut.Input.Path);
            }

            return isTimeEvent;
        }
    }
}