using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Duelity
{
    public class DuelMiniGame
    {
        public float Value { get; private set; }
        public float MaxSpeed { get; private set; }
        public float Speed { get; private set; }
        public int SignOfDirection { get; private set; }
        public DuelMiniGameConfig Config { get; }

        float _elapsedTime;

        List<FloatRange> _floatRanges;

        HashSet<int> _targetRangeIndices;

        public List<FloatRange> FloatRanges => _floatRanges;

        public HashSet<int> TargetRangeIndices => _targetRangeIndices;

        public DuelMiniGame(DuelMiniGameConfig config)
        {
            Config = config;
            MaxSpeed = Config.StartingSpeedRange.GetRandomValueInsideRange();
            Speed = MaxSpeed;
            SignOfDirection = (int)Mathf.Sign(Config.StartingDirection);

            Value = UnityEngine.Random.value;
            const int magicNumber = 6;
            const float chunkSize = 1f / magicNumber;
            _floatRanges = new List<FloatRange>();
            for (int i = 0; i < magicNumber; i++)
            {
                var range = new FloatRange(i * chunkSize, (i + 1) * chunkSize);
                _floatRanges.Add(range);
            }

            _targetRangeIndices = UnityEngine.Random.value >= .5f
                ? new HashSet<int>() { 0, 2, 4 }
                : new HashSet<int>() { 0, 2, 4 };
        }

        public void FlipDirection() => SignOfDirection *= -1;

        public bool IsInsideValidRange(float value, out FloatRange floatRange)
        {
            floatRange = _floatRanges.First(r => r.ValueIsInsideRange(value));
            int index = _floatRanges.IndexOf(floatRange);
            return _targetRangeIndices.Contains(index);
        }

        public bool IsInsideValidRange(out FloatRange floatRange)
        {
            return IsInsideValidRange(Value, out floatRange);
        }

        public bool RemoveRange(FloatRange floatRange, out int indexOfRemoved)
        {
            indexOfRemoved = _floatRanges.IndexOf(floatRange);
            bool removed = _targetRangeIndices.Remove(indexOfRemoved);

            float chance = Config.DirectionChangeChance.GetRandomValueInsideRange();
            if (UnityEngine.Random.value <= chance)
            {
                FlipDirection();
            }

            return removed;
        }

        public void Update(float deltaTime)
        {
            Value += (Speed * deltaTime * SignOfDirection);
            if (Value > 1f)
            {
                Value -= 1f;
            }
            else if (Value < 0)
            {
                Value = 1f - Value;
            }

            _elapsedTime += deltaTime;
            float speedT = Config.SlowDownOverTime.Evaluate(Math.Min(1, _elapsedTime / Config.SlowDownDuration));
            Speed = Mathf.Lerp(Config.MinSpeed, MaxSpeed, speedT);
        }
    }
}
