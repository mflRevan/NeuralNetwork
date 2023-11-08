using System;
using System.Collections.Generic;
using UnityEngine;


namespace Default
{
    [CreateAssetMenu(fileName = "TraniningDatabase", menuName = "AI/TrainingDatabase")]
    public class TrainingDatabase : ScriptableObject
    {
        public List<MapData> Maps;
    }

    [Serializable]
    public class MapData
    {
        public List<int> list;

        public MapData()
        {
            list = new();
        }
    }
}