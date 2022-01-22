using System;

namespace Duelity.Utility
{
    public static class EventExtensions
    {
        public static void ListenTo<T>(this object listenerObject,
                               Event<T> event0,
                               Action<T> action,
                               Events.ListenerPriority listenerPriority = Events.ListenerPriority.Default)
        {
            event0.AddListener(listenerObject, action, listenerPriority);
        }

        public static void StopListeningTo<T>(this object listenerObject, T @event) where T : IEvent
        {
            @event.RemoveListener(listenerObject);
        }
    }
}