using System;
using System.Collections.Generic;
using UnityEngine;

namespace NicoFramework.Tools.EventCenter.Trigger
{
    public class EventDestroyTrigger : MonoBehaviour
    {
        public bool IsCalledOnDestroy { get; } = false;

        private List<IDisposable> _disposablesOnDestroy;

        private void OnDestroy()
        {
            if (!IsCalledOnDestroy) {
                if (_disposablesOnDestroy != null) {
                    foreach (var dispose in _disposablesOnDestroy) {
                        dispose.Dispose();
                    }
                }
            }
        }

        public void AddDisposableOnDestroy(IDisposable disposable)
        {
            if (IsCalledOnDestroy) {
                disposable.Dispose();
                return;
            }

            if (_disposablesOnDestroy == null) {
                _disposablesOnDestroy = new List<IDisposable>();
            }

            _disposablesOnDestroy.Add(disposable);
        }
    }
}