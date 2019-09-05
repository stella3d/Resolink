using System.Collections.Generic;
using UnityEngine;

namespace Resolink
{
    /// <summary>
    /// Used to store data about the data received for each event from Resolume
    /// </summary>
    [CreateAssetMenu]
    public class ResolumeEventMetaData : ScriptableObject
    {
        /// <summary>
        /// The type for each path in InputPaths
        /// </summary>
        public List<TypeSelectionEnum> Types;

        /// <summary>
        /// All input paths (and regular expressions of paths) to use in trying to map shortcuts to types.
        /// </summary>
        public List<string> InputPaths;

        void OnEnable()
        {
            if(Types == null)
                Types = new List<TypeSelectionEnum>();
            if(InputPaths == null)
                InputPaths = new List<string>();
        }

        public void AddCapacity(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Types.Add(default);
                InputPaths.Add(default);
            }
        }

        public void Trim(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                Types.RemoveAt(Types.Count - 1);
                InputPaths.RemoveAt(InputPaths.Count - 1);
            }
        }
    }
}