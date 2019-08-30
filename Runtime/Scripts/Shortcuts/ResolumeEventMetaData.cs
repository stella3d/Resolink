using System.Collections.Generic;
using UnityEngine;

namespace Resolunity
{
    [CreateAssetMenu]
    public class ResolumeEventMetaData : ScriptableObject
    {
        public List<TypeSelectionEnum> Types;

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