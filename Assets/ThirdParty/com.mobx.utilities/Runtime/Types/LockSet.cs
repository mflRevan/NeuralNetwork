using System;
using System.Collections.Generic;

namespace MobX.Utilities
{
    public class LockSet
    {
        public event Action Locked;
        public event Action Unlocked;
        public bool IsLocked { get; private set; }

        private readonly HashSet<object> _locks = new();

        public void AddLock(object key)
        {
            var wasLocked = _locks.Count > 0;
            _locks.Add(key);
            if (wasLocked)
            {
                IsLocked = true;
                Locked?.Invoke();
            }
        }

        public void RemoveLock(object key)
        {
            if (_locks.Remove(key) && _locks.Count <= 0)
            {
                IsLocked = false;
                Unlocked?.Invoke();
            }
        }

        public void ReleaseAllLocks()
        {
            var wasLocked = _locks.Count > 0;
            _locks.Clear();
            if (wasLocked)
            {
                IsLocked = false;
                Unlocked?.Invoke();
            }
        }
    }
}
