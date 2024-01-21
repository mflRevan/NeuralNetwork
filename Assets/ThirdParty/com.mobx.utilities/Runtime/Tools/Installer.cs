using MobX.Utilities.Callbacks;
using MobX.Utilities.Inspector;
using MobX.Utilities.Types;
using System;
using UnityEngine;

namespace MobX.Utilities.Tools
{
    public class Installer : RuntimeAsset, IOnBeforeFirstSceneLoad, IOnQuit
    {
        [SerializeField] private bool autoInstall;
        [ListOptions]
        [SerializeField] private Optional<GameObject>[] systems;

        [NonSerialized] private bool _installed;

        public void Install()
        {
            if (_installed)
            {
                return;
            }
            _installed = true;
            foreach (var system in systems)
            {
                if (system.TryGetValue(out var prefab))
                {
                    var instance = Instantiate(prefab);
                    instance.DontDestroyOnLoad();
                }
            }
        }

        public void OnBeforeFirstSceneLoad()
        {
            if (autoInstall)
            {
                Install();
            }
        }

        public void OnQuit()
        {
            _installed = false;
        }
    }
}