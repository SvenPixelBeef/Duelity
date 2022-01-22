using System;
using UnityEngine;

namespace Duelity
{
    public delegate T RandomInsideRange<T>(T min, T max) where T : struct, IComparable, IComparable<T>;

    [Serializable]
    public abstract class Range<T>
        where T : struct, IComparable, IComparable<T>
    {
        [SerializeField] protected T _min;
        public T Min => _min;

        [SerializeField] protected T _max;
        public T Max => _max;

        public Range() { }

        public Range(T min, T max)
        {
            _min = min;
            _max = max;
        }

        protected abstract RandomInsideRange<T> RandomInsideRange { get; }

        /// <summary>
        /// Min is inclusive, Max is exclusive
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ValueIsInsideRange(T value)
        {
            bool largerThenMin = value.CompareTo(_min) >= 0;
            bool smallerThenMax = value.CompareTo(_max) < 0;
            return largerThenMin && smallerThenMax;
        }

        public T GetRandomValueInsideRange() => RandomInsideRange(_min, _max);
    }

    [Serializable]
    public class FloatRange : Range<float>
    {
        protected override RandomInsideRange<float> RandomInsideRange => UnityEngine.Random.Range;

        public FloatRange(float min, float max) : base(min, max) { }
    }

    [Serializable]
    public class IntRange : Range<int>
    {
        protected override RandomInsideRange<int> RandomInsideRange => UnityEngine.Random.Range;

        public IntRange(int min, int max) : base(min, max) { }
    }
}
