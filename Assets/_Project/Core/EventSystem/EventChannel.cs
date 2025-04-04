using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public abstract class EventChannel<T> : ScriptableObject
    {
        readonly HashSet<EventListener<T>> observers = new();

        public void Invoke(T value)
        {
            foreach (var observer in observers)
            {
                observer.Raise(value);
            }
        }

        public void Register(EventListener<T> observer) => observers.Add(observer);
        public void Unregister(EventListener<T> observer) => observers.Remove(observer);
    }

    public readonly struct Empty { }
    [CreateAssetMenu(menuName = "Event/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
}
