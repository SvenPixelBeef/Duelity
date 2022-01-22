using System.Collections.Generic;
using System;
using UnityEngine;

namespace Duelity.Utility
{
    public class Event<T> : IEvent
    {
        List<EventListener<T>> _eventListeners;

        Dictionary<int, EventListener<T>> _eventListenersDict;

        bool _sorted;

        public Event()
        {
            _eventListeners = new List<EventListener<T>>();
            _eventListenersDict = new Dictionary<int, EventListener<T>>();
        }

        public void AddListener(object listenerObject,
                                Action<T> action,
                                Events.ListenerPriority listenerPriority = Events.ListenerPriority.Default)
        {
            Debug.Assert(action != null);

            int key = listenerObject.GetHashCode();
            bool alreadyRegistered = _eventListenersDict.ContainsKey(key);
            Debug.Assert(!alreadyRegistered);
            if (alreadyRegistered)
                return;

            EventListener<T> eventListener = new EventListener<T>(listenerObject, action, listenerPriority);
            _eventListeners.Add(eventListener);
            _eventListenersDict.Add(listenerObject.GetHashCode(), eventListener);

            _sorted = false;
        }

        public void RemoveListener(object listenerObject)
        {
            Debug.Assert(listenerObject != null);

            int key = listenerObject.GetHashCode();
            bool isListener = _eventListenersDict.TryGetValue(key, out EventListener<T> _listener);
            Debug.Assert(isListener);
            if (!isListener)
                return;

            _eventListeners.Remove(_listener);
            _eventListenersDict.Remove(key);
        }


        public bool TryRemoveListener(object listenerObject)
        {
            int key = listenerObject.GetHashCode();
            if (_eventListenersDict.TryGetValue(key, out EventListener<T> _listener))
            {
                _eventListeners.Remove(_listener);
                _eventListenersDict.Remove(key);
                return true;
            }

            return false;
        }

        public void RemoveAllListeners()
        {
            _eventListeners.Clear();
            _eventListenersDict.Clear();
        }

        public void RaiseEvent(T arg)
        {
            if (!_sorted)
            {
                _eventListeners.Sort();
                _sorted = true;
            }

            int actionCount = _eventListeners.Count;
            for (int i = actionCount - 1; i >= 0; i--)
            {
                if (!_eventListeners[i].IsValid)
                {
                    int key = _eventListeners[i].ListenerObject.GetHashCode();
                    Debug.Assert(_eventListenersDict.ContainsKey(key));

                    _eventListenersDict.Remove(key);
                    _eventListeners.RemoveAt(i);

                    continue;
                }

                _eventListeners[i].Action.Invoke(arg);
            }
        }
    }
}