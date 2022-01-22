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


        [SerializeField, Space] float _fadeInDuration = 0.5f;
        public float FadeInDuration => _fadeInDuration;

        [SerializeField] float _fadeOutDuration = 0.5f;
        public float FadeOutDuration => _fadeOutDuration;

        [SerializeField, Space] float _requiredDurationSecretEnding = 30f;
        public float RequiredDurationSecretEnding => _requiredDurationSecretEnding;

        [SerializeField, Space] float _walkAwaySpeed = 3f;
        public float WalkAwaySpeed => _walkAwaySpeed;

        [Header("Sounds")]
        [Space]

        [SerializeField, Expandable] Sound _gunShotSound;
        public Sound GunShotSound => _gunShotSound;

        [SerializeField, Expandable] List<Sound> _reloadSounds;
        public IReadOnlyCollection<Sound> ReloadSounds => _reloadSounds;

        [Header("Music")]
        [Space]

        [SerializeField, Expandable] Sound _titleScreenMusic;
        public Sound TitleScreenMusic => _titleScreenMusic;

        [SerializeField, Expandable] Sound _ambienceMusic;
        public Sound AmbienceMusic => _ambienceMusic;
    }
}
