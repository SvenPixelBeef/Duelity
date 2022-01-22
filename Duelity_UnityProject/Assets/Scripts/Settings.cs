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

        [SerializeField, Expandable] DuelMiniGameConfig _duelMiniGameConfig;
        public DuelMiniGameConfig DuelMiniGameConfig => _duelMiniGameConfig;

    }
}
