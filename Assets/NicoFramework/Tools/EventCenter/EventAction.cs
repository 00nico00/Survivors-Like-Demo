using System;
using System.Collections.Generic;

namespace NicoFramework.Tools.EventCenter
{
    /// <summary>
    /// 包裹一下要 Dispose 的行为
    /// </summary>
    public class EventAction : IDisposable
    {
        private KeyValuePair<Type, Action<object>> _typeActionPair;

        public EventAction(Type type, Action<object> notifier)
        {
            _typeActionPair = new KeyValuePair<Type, Action<object>>(type, notifier);
        }
        
        public void Dispose()
        {
            EventCenter.Default.Remove(_typeActionPair.Key, _typeActionPair.Value);
        }
    }
}