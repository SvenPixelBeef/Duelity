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


        [SerializeField, Space] float _requiredDurationSecretEnding = 30f;
        public float RequiredDurationSecretEnding => _requiredDurationSecretEnding;

        [SerializeField, Space] float _walkAwaySpeed = 3f;
        public float WalkAwaySpeed => _walkAwaySpeed;


        [SerializeField, Space] float _birdReactionDelay = .33f;
        public float BirdReactionDelay => _birdReactionDelay;


        [SerializeField, Space] Color _failColor;
        public Color FailColor => _failColor;

        [Header("Fading in and out")]
        [Space]

        [SerializeField, Space] float _fadeInDuration = 0.5f;
        public float FadeInDuration => _fadeInDuration;

        [SerializeField] AnimationCurve _fadeInCurve;
        public AnimationCurve FadeInCurve => _fadeInCurve;

        [SerializeField, Space] float _fadeOutDuration = 0.5f;
        public float FadeOutDuration => _fadeOutDuration;

        [SerializeField] AnimationCurve _fadeOutCurve;
        public AnimationCurve FadeOutCurve => _fadeOutCurve;



        [Header("Sounds")]
        [Space]

        [SerializeField, Expandable] Sound _gunShotSound;
        public Sound GunShotSound => _gunShotSound;

        [SerializeField, Expandable] List<Sound> _reloadSounds;
        public IReadOnlyCollection<Sound> ReloadSounds => _reloadSounds;

        [SerializeField, Expandable] Sound _crowsSound;
        public Sound CrowsSound => _crowsSound;

        [Header("Music")]
        [Space]

        [SerializeField, Expandable] Sound _titleScreenMusic;
        public Sound TitleScreenMusic => _titleScreenMusic;

        [SerializeField, Expandable] Sound _ambienceMusic;
        public Sound AmbienceMusic => _ambienceMusic;

        [SerializeField, Expandable] Sound _ambienceMusic2;
        public Sound AmbienceMusic2 => _ambienceMusic2;


        [Header("Screen shake")]

        [SerializeField, Expandable] CameraShake _cameraShakeShot;
        public CameraShake CameraShakeShot => _cameraShakeShot;

        [SerializeField, Expandable] CameraShake _cameraShakeReloadSuccess;
        public CameraShake CameraShakeReloadSuccess => _cameraShakeReloadSuccess;

        [SerializeField, Expandable] CameraShake _cameraShakeReloadFail;
        public CameraShake CameraShakeReloadFail => _cameraShakeReloadFail;
    }
}
