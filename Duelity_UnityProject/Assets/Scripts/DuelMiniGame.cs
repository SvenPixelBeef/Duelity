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

        List<FloatRange> _validFloatRanges;

        public IReadOnlyCollection<FloatRange> ValidFloatRanges => _validFloatRanges;

        public DuelMiniGame(DuelMiniGameConfig config)
        {
            Config = config;
            MaxSpeed = Config.StartingSpeedRange.GetRandomValueInsideRange();
            Speed = MaxSpeed;
            SignOfDirection = (int)Mathf.Sign(Config.StartingDirection);

            Value = UnityEngine.Random.value;
            const int magicNumber = 6;
            const float chunkSize = 1f / magicNumber;
            _validFloatRanges = new List<FloatRange>();
            for (int i = 0; i < magicNumber; i++)
            {
                var range = new FloatRange(i * chunkSize, (i + 1) * chunkSize);
                _validFloatRanges.Add(range);
            }

            // TODO: make this less garbage
            var indicesToRemove = new HashSet<int>();
            while (indicesToRemove.Count < (magicNumber / 2))
            {
                int toRemove = UnityEngine.Random.Range(0, magicNumber);
                indicesToRemove.Add(toRemove);
            }

            foreach (var index in indicesToRemove)
            {
                _validFloatRanges[index] = null;
            }

            _validFloatRanges.RemoveAll((r => r == null));
        }

        public void FlipDirection() => SignOfDirection *= -1;

        public bool IsInsideValidRange(float value, out FloatRange floatRange)
        {
            floatRange = _validFloatRanges.FirstOrDefault(r => r.ValueIsInsideRange(value));
            return floatRange != null;
        }

        public bool IsInsideValidRange(out FloatRange floatRange)
        {
            return IsInsideValidRange(Value, out floatRange);
        }

        public bool RemoveRange(FloatRange floatRange)
        {
            return _validFloatRanges.Remove(floatRange);
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
