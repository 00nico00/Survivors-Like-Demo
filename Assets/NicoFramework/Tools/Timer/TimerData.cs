using System;

namespace NicoFramework.Tools.Timer
{
    public class TimerData
    {
        public int timerId;
        public double duration;
        public int loopTimes = 1;
        public bool realtimeUpdate = false;
        public bool isSetDuration = false;

        public long startDateTime;
    }
}