using System;
using UnityEngine;

namespace Duelity
{
    [CreateAssetMenu(fileName = "DuelMiniGameConfig", menuName = "Duelity/DuelMiniGameConfig")]
    public class DuelMiniGameConfig : ScriptableObject
    {
        [SerializeField] FloatRange _startingSpeedRange;
        public FloatRange StartingSpeedRange => _startingSpeedRange;

        [SerializeField] int _startingDirection;
        public int StartingDirection
        {
            get
            {
                return _randomizeStartingDirection
                    ? UnityEngine.Random.value > .5f ? 1 : -1
                    : _startingDirection;
            }
        }

        [SerializeField] bool _randomizeStartingDirection;
        public bool RandomizeStartingDirection => _randomizeStartingDirection;


        [SerializeField, Space] AnimationCurve _slowDownOverTime;
        public AnimationCurve SlowDownOverTime => _slowDownOverTime;

        [SerializeField] float _slowDownDuration;
        public float SlowDownDuration => _slowDownDuration;

        [SerializeField] float _minSpeed;
        public float MinSpeed => _minSpeed;


        [SerializeField, Space] FloatRange _directionChangeChance;
        public FloatRange DirectionChangeChance => _directionChangeChance;
    }
}
