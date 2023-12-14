using System;
using UnityEngine;

namespace NicoFramework.Tools.Timer
{
    public class Timer
    {
        private TimerData _timerData;
        private double _totalDuration;
        private double _elapsedTime = 0.0;
        private double _totalElapsedTime = 0.0;
        private int _currentLoopCount = 0;

        private Action _onEveryLoopFinish;
        private Action _onFinalLoopFinish;
        private Action _onCreate;

        public int TimerId => _timerData.timerId;
        public double RestTime => _totalDuration - _totalElapsedTime;

        public string RestTimeFormat_HMS
        {
            get
            {
                var ts = TimeSpan.FromSeconds(RestTime);
                return $"{ts.TotalHours:00}:{ts.Minutes:d2}:{ts.Seconds:d2}";
            }
        }

        public string RestTimeFormat_MS
        {
            get
            {
                var ts = TimeSpan.FromSeconds(RestTime);
                return $"{ts.TotalMinutes:00}:{ts.Seconds:d2}";
            }
        }

        public string RestTimeFormat_HM
        {
            get
            {
                var ts = TimeSpan.FromSeconds(RestTime);
                return $"{ts.TotalHours:00}:{ts.Minutes:d2}";
            }
        }

        public Timer()
        {
            _timerData = new TimerData();
            _onFinalLoopFinish += UnRegister;
        }

        public Timer(TimerData timerData)
        {
            _timerData = timerData;
            _onFinalLoopFinish += UnRegister;
        }

        public Timer(int timerId)
        {
            _timerData = new TimerData()
            {
                timerId = timerId
            };
            _onFinalLoopFinish += UnRegister;
        }

        public void OnUpdate()
        {
            _totalElapsedTime += _timerData.realtimeUpdate ? Time.unscaledDeltaTime : Time.deltaTime;
            if (_elapsedTime < _timerData.duration) {
                _elapsedTime += _timerData.realtimeUpdate ? Time.unscaledDeltaTime : Time.deltaTime;
            } else {
                _elapsedTime = 0.0;
                _currentLoopCount++;
                _onEveryLoopFinish?.Invoke();

                if (_currentLoopCount >= _timerData.loopTimes) {
                    _onFinalLoopFinish?.Invoke();
                }
            }
        }

        public Timer SetDuration(TimeSpan duration)
        {
            _timerData.duration = duration.TotalSeconds;
            _timerData.isSetDuration = true;
            UpdateTotalDuration();
            return this;
        }

        public Timer SetLoopTime(int loopTimes)
        {
            _timerData.loopTimes = loopTimes;
            UpdateTotalDuration();
            return this;
        }

        public Timer SetRealtimeUpdate(bool realTimeUpdate)
        {
            _timerData.realtimeUpdate = realTimeUpdate;
            return this;
        }

        public Timer OnEveryLoopFinish(Action onEveryLoopFinishCallback)
        {
            _onEveryLoopFinish += onEveryLoopFinishCallback;
            return this;
        }

        public Timer OnFinalLoopFinish(Action onFinalLoopFinishCallback)
        {
            _onFinalLoopFinish += onFinalLoopFinishCallback;
            return this;
        }

        public Timer OnCrate(Action onCreateCallback)
        {
            _onCreate += onCreateCallback;
            return this;
        }

        public void Register()
        {
            if (!_timerData.isSetDuration) {
                Debug.LogError("没有设置计时器的 duration");
            }

            _onCreate?.Invoke();

            TimerManager.Instance.RegisterTimer(this);
        }

        public void UnRegister()
        {
            TimerManager.Instance.UnRegisterTimer(this);
        }

        private void UpdateTotalDuration()
        {
            _totalDuration = _timerData.duration * _timerData.loopTimes;
        }
    }
}