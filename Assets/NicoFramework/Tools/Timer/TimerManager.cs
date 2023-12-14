using System;
using System.Collections.Generic;
using UnityEngine;

namespace NicoFramework.Tools.Timer
{
    public class TimerManager : MonoBehaviour
    {
        public static TimerManager Instance { get; private set; }

        private List<Timer> _timers = new List<Timer>();

        private void Awake()
        {
            if (Instance != null) {
                Destroy(gameObject);
            }

            Instance = this;
        }

        private void Update()
        {
            for (int i = 0; i < _timers.Count; i++) {
                _timers[i].OnUpdate();
            }
        }

        public Timer CreateTimer()
        {
            var timer = new Timer();
            return timer;
        }


        public void RegisterTimer(Timer timer)
        {
            _timers.Add(timer);
        }

        public void UnRegisterTimer(Timer timer)
        {
            _timers.Remove(timer);
        }
    }
}