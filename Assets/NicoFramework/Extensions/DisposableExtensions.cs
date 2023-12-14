using System;
using NicoFramework.Tools.EventCenter.Trigger;
using UnityEngine;

namespace NicoFramework.Extensions
{
    public static class DisposableExtensions
    {
        public static T BindLifetime<T>(this T disposable, Component gameObjectComponent)
            where T: IDisposable
        {
            if (gameObjectComponent == null) {
                disposable.Dispose();
                return disposable;
            }

            return BindLifetime(disposable, gameObjectComponent.gameObject);
        }

        public static T BindLifetime<T>(this T disposable, GameObject gameObject)
            where T: IDisposable
        {
            if (gameObject == null) {
                disposable.Dispose();
                return disposable;
            }

            var trigger = gameObject.GetComponent<EventDestroyTrigger>();
            if (trigger == null) {
                trigger = gameObject.AddComponent<EventDestroyTrigger>();
            }
            
            trigger.AddDisposableOnDestroy(disposable);

            return disposable;
        }
    }
}