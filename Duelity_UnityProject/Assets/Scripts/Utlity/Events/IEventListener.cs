using System;

namespace Duelity.Utility
{
    public interface IEventListener : IComparable<IEventListener>
    {
        Events.ListenerPriority ListenerPriority { get; }

        object ListenerObject { get; }

        bool IsValid { get; }
    }
}