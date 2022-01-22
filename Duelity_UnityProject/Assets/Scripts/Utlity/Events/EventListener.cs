using System;

namespace Duelity.Utility
{
    public class EventListener<T> : IEventListener
    {
        readonly Action<T> _action;

        readonly Events.ListenerPriority _listenerPriority;

        readonly object _listenerObject;


        public Action<T> Action => _action;
        public Events.ListenerPriority ListenerPriority => _listenerPriority;
        public object ListenerObject => _listenerObject;

        public bool IsValid => _action != null;


        public EventListener(object listenerObject, Action<T> action, Events.ListenerPriority listenerPriority)
        {
            _listenerObject = listenerObject;
            _action = action;
            _listenerPriority = listenerPriority;
        }

        public int CompareTo(IEventListener other)
        {
            return _listenerPriority.CompareTo(other.ListenerPriority);
        }
    }
}