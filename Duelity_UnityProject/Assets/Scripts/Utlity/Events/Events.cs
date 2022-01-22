namespace Duelity.Utility
{
    public partial class Events
    {
        public enum ListenerPriority
        {
            Lowest = -100,
            Low = -50,

            Default = 0,

            High = 50,
            Highest = 100,
        }

        // NoEventArgs is used for all events that do not send any data
        public readonly struct NoEventArgs { }
        public static NoEventArgs NoArgs { get; } = new NoEventArgs();


        public static Event<NoEventArgs> DuelStarted { get; }
            = new Event<NoEventArgs>();

    }
}