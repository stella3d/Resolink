using System.Collections.Generic;
using UnityEngine;

namespace Resolunity
{
    public class ResolumeEventMetaData : ScriptableObject
    {
        [SerializeField] List<long> UniqueIds;

        [SerializeField] List<string> OutputPaths;
    }
}