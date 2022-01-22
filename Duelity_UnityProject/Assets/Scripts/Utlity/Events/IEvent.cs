namespace Duelity.Utility
{
    public interface IEvent
    {
        void RemoveListener(object listenerObject);
        bool TryRemoveListener(object listenerObject);
        void RemoveAllListeners();
    }
}