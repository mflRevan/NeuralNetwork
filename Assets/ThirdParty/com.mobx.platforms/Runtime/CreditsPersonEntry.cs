using System;
using UnityEngine;

namespace MobX.Platforms
{
    [Serializable]
    public class CreditsPersonEntry
    {
        [SerializeField] private string role;
        [SerializeField] private string name;

        public string Role => role;
        public string Name => name;
    }
}
