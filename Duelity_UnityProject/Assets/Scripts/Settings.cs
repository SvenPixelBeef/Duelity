using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duelity
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Duelity/Settings")]
    public class Settings : ScriptableObject
    {
        [SerializeField] KeyCode _keyCodeLeftPlayer;
        public KeyCode KeyCodeLeftPlayer => _keyCodeLeftPlayer;


        [SerializeField] KeyCode _keyCodeRightPlayer;
        public KeyCode KeyCodeRightPlayer => _keyCodeRightPlayer;

        [SerializeField, Space] FloatRange _dualStartTimeRange;
        public FloatRange DualStartTimeRange => _dualStartTimeRange;


        [SerializeField, Range(0f, 1f), Space] float _dualTimeScale = 0.1f;
        public float DualTimeScale => _dualTimeScale;

        [SerializeField, Space, Expandable] DuelMiniGameConfig _duelMiniGameConfig;
        public DuelMiniGameConfig DuelMiniGameConfig => _duelMiniGameConfig;

    }
}
