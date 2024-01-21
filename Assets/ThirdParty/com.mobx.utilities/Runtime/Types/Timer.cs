using UnityEngine;

namespace MobX.Utilities.Types
{
    public readonly struct Timer
    {
        // State
        private readonly float _targetTime;
        private readonly float _startTime;

        public Timer(float delayInSeconds)
        {
            _startTime = Time.time;
            _targetTime = _startTime + delayInSeconds;
        }

        public bool IsRunning => _targetTime > Time.time;

        public bool Expired => 0 < _targetTime && _targetTime <= Time.time;

        public bool ExpiredOrNotRunning => _targetTime <= Time.time;

        public float? RemainingTime => IsRunning ? _targetTime - Time.time : null;

        public float Delta(float fallback = 0)
        {
            if (IsRunning)
            {
                var totalDuration = _targetTime - _startTime;
                var passedTime = totalDuration - RemainingTime!.Value;
                return passedTime / totalDuration;
            }
            return fallback;
        }

        public override string ToString()
        {
            return $"{nameof(Timer)}: {_targetTime}";
        }

        public static Timer None => new Timer();
    }
}
