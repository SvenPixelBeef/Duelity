using Duelity.Utility;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Duelity
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Expandable] Settings _settings;

        [Header("Scene References")]

        [SerializeField] Player _leftPlayer;
        [SerializeField] Player _rightPlayer;

        [SerializeField] DuelMiniGameDisplay _leftPlayerDuelMiniGameDisplay;
        [SerializeField] DuelMiniGameDisplay _rightPlayerDuelMiniGameDisplay;

        [SerializeField] SpriteRenderer _fadeOutSpriteRenderer;

        [SerializeField] TextMeshPro _titleText;
        [SerializeField] TextMeshPro _anyKeyPromptText;

        [SerializeField] ParticleSystem _birdsPS;

        [SerializeField] Shaker _cameraShaker;


        Color FadeOutColor => Color.black;
        Color FadeInColor => new Color(0f, 0f, 0f, 0f);

        Coroutine _crossFadeCoroutine;

        Coroutine _endingCoroutine;

        bool _anyEndingWasTriggered;

        static AudioManager _audio;

        public static Game Instance { get; private set; }

        public static Shaker CameraShaker => Instance._cameraShaker;
        public static Settings Settings => Instance._settings;
        public static AudioManager Audio => _audio ??= new AudioManager(10);

        public static ParticleSystem BirdsPS => Instance._birdsPS;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Init()
        {
            _audio = null;
        }

        void Awake()
        {
            Instance = this;
            this.ListenTo(Events.PlayerReloadedAll, OnPlayerReloadedAll);
            this.ListenTo(Events.PlayerFailedReload, OnPlayerFailedReload);

            _anyKeyPromptText.enabled = false;
            _titleText.enabled = false;
#if !UNITY_EDITOR
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
#endif
        }

        IEnumerator Start()
        {
            Settings.TitleScreenMusic.Play(target: this);
            FadeIn(Settings.FadeInDuration);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            FadeInText(_titleText, Settings.FadeInDuration, null);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            _anyKeyPromptText.enabled = true;
            _anyKeyPromptText.GetComponent<Animation>().Play();

            yield return new WaitUntil(() => Input.anyKeyDown);
            // TODO: Play feedback sound here

            _anyKeyPromptText.GetComponent<Animation>().Stop();
            FadeOutText(_anyKeyPromptText, 2f, null);
            FadeOutText(_titleText, 2f, null);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            float musicFadeDuration = 1.5f;
            Settings.AmbienceMusic.Play(this, 0f, 1f);
            Settings.AmbienceMusic.Fade(this, Settings.AmbienceMusic.Volume, musicFadeDuration, true);
            Settings.TitleScreenMusic.Fade(this, 0f, musicFadeDuration, true);

            Settings.AmbienceMusic2.Play(this, 0f, 1f);
            Settings.AmbienceMusic2.Fade(this, Settings.AmbienceMusic2.Volume, musicFadeDuration, true);
            // yield return new WaitForSecondsRealtime(musicFadeDuration);


            float duelTriggerTime = Settings.DualStartTimeRange.GetRandomValueInsideRange();
            yield return new WaitForSecondsRealtime(duelTriggerTime);

            Events.DuelStarted.RaiseEvent(Events.NoArgs);
            _leftPlayerDuelMiniGameDisplay.PlayEnterAnimation();
            _rightPlayerDuelMiniGameDisplay.PlayEnterAnimation();


            // TODO: Play more/other sounds/music here?


            yield return new WaitForSecondsRealtime(Settings.RequiredDurationSecretEnding);
            if (!_anyEndingWasTriggered)
            {
                Events.SecretEndingTriggered.RaiseEvent(Events.NoArgs);
                _anyEndingWasTriggered = true;
                _leftPlayerDuelMiniGameDisplay.PlayExitAnimation();
                _rightPlayerDuelMiniGameDisplay.PlayExitAnimation();
                _endingCoroutine = StartCoroutine(SecretEndingRoutine());

            }
        }

        void OnDestroy()
        {
            this.StopListeningTo(Events.PlayerReloadedAll);
            this.StopListeningTo(Events.PlayerFailedReload);
        }

        void Update()
        {
            DebugStuff();
        }

        void OnPlayerFailedReload(Player playerWhoFailed)
        {
            Debug.Assert(_endingCoroutine == null);
            _endingCoroutine = StartCoroutine(PlayerFailedEndingRoutine());
            IEnumerator PlayerFailedEndingRoutine()
            {
                _anyEndingWasTriggered = true;
                playerWhoFailed.Fumble();
                yield return new WaitForSecondsRealtime(1f);
                _leftPlayerDuelMiniGameDisplay.PlayExitAnimation();
                _rightPlayerDuelMiniGameDisplay.PlayExitAnimation();

                Player winningPlayer = playerWhoFailed == _leftPlayer ? _rightPlayer : _leftPlayer;
                bool winningPlayerShot = false;
                bool mercy = false;
                float mercyTimer = 0f;
                while (!winningPlayerShot && !mercy)
                {
                    winningPlayerShot = Input.GetKeyDown(winningPlayer.ActionKey);
                    mercyTimer += Time.unscaledDeltaTime;
                    mercy = mercyTimer >= 7f;
                    yield return null;
                }

                if (winningPlayerShot)
                {
                    winningPlayer.Shoot();
                    yield return null;
                    playerWhoFailed.Die();

                    yield return new WaitForSecondsRealtime(2f);

                    winningPlayer.WalkAway();

                    yield return new WaitForSecondsRealtime(3.5f);
                    FadeInText(_titleText, 2f, null);
                    yield return new WaitForSecondsRealtime(2f);

                    yield return new WaitUntil(() => Input.anyKeyDown);
                    Reload();
                }
                // mercy
                else
                {
                    StartCoroutine(SecretEndingRoutine());
                }
            }
        }

        void OnPlayerReloadedAll(Player winningPlayer)
        {
            Debug.Assert(_endingCoroutine == null);
            _endingCoroutine = StartCoroutine(PlayerSuccededEndingRoutine());
            IEnumerator PlayerSuccededEndingRoutine()
            {
                _leftPlayerDuelMiniGameDisplay.PlayExitAnimation();
                _rightPlayerDuelMiniGameDisplay.PlayExitAnimation();
                _anyEndingWasTriggered = true;
                winningPlayer.Shoot();
                yield return null;
                Player losingPlayer = _leftPlayer == winningPlayer ? _rightPlayer : _leftPlayer;
                losingPlayer.Die();

                yield return new WaitForSecondsRealtime(2f);

                winningPlayer.WalkAway();

                yield return new WaitForSecondsRealtime(3.5f);
                FadeInText(_titleText, 2f, null);
                yield return new WaitForSecondsRealtime(2f);

                yield return new WaitUntil(() => Input.anyKeyDown);
                Reload();
            }
        }

        void FadeIn(float duration, Action callback = null)
        {
            _fadeOutSpriteRenderer.color = Color.black;
            Fade(FadeOutColor, FadeInColor, duration, Settings.FadeInCurve, callback);
        }

        void FadeOut(float duration, Action callback = null)
        {
            var color = Color.black;
            color.a = 0f;
            _fadeOutSpriteRenderer.color = color;
            Fade(FadeInColor, FadeOutColor, duration, Settings.FadeOutCurve, callback);
        }

        void Fade(Color startingColor, Color targetColor, float duration, AnimationCurve curve = null, Action callback = null)
        {
            if (_crossFadeCoroutine != null)
            {
                StopCoroutine(_crossFadeCoroutine);
                _crossFadeCoroutine = null;
            }

            _crossFadeCoroutine
                = StartCoroutine(ColorLerpRoutine(startingColor, targetColor, duration, (color) => _fadeOutSpriteRenderer.color = color, curve, callback));
        }

        public void FadeOutText(TextMeshPro text, float duration, AnimationCurve curve, Action doneCallback = null)
        {
            text.enabled = true;
            var baseColor = text.color;
            var startingColor = baseColor;
            startingColor.a = 1f;
            var targetColor = baseColor;
            targetColor.a = 0f;
            StartCoroutine(ColorLerpRoutine(startingColor, targetColor, duration, (color) => text.color = color, curve, doneCallback));
        }

        public void FadeInText(TextMeshPro text, float duration, AnimationCurve curve, Action doneCallback = null)
        {
            text.enabled = true;
            var baseColor = text.color;
            var startingColor = baseColor;
            startingColor.a = 0f;
            var targetColor = baseColor;
            targetColor.a = 1f;
            StartCoroutine(ColorLerpRoutine(startingColor, targetColor, duration, (color) => text.color = color, curve, doneCallback));
        }

        public void FadeInSpriteRenderer(SpriteRenderer spriteRenderer, float duration, AnimationCurve curve, Action doneCallback = null)
        {
            spriteRenderer.enabled = true;
            var baseColor = spriteRenderer.color;
            var startingColor = baseColor;
            startingColor.a = 0f;
            var targetColor = baseColor;
            targetColor.a = 1f;
            StartCoroutine(ColorLerpRoutine(startingColor, targetColor, duration, (color) => spriteRenderer.color = color, curve, doneCallback));
        }

        public void FadeOutSpriteRenderer(SpriteRenderer spriteRenderer, float duration, AnimationCurve curve, Action doneCallback = null)
        {
            spriteRenderer.enabled = true;
            var baseColor = spriteRenderer.color;
            var startingColor = baseColor;
            startingColor.a = 1f;
            var targetColor = baseColor;
            targetColor.a = 0f;
            StartCoroutine(ColorLerpRoutine(startingColor, targetColor, duration, (color) => spriteRenderer.color = color, curve, doneCallback));
        }

        public IEnumerator ColorLerpRoutine(Color startingColor,
            Color targetColor,
            float duration,
            Action<Color> callback,
            AnimationCurve curve,
            Action doneCallback = null)
        {
            duration = Math.Max(Mathf.Epsilon, duration);
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float progress = elapsedTime / duration;
                float t = curve?.Evaluate(progress) ?? progress;
                Color color = Color.Lerp(startingColor, targetColor, t);
                callback.Invoke(color);
                yield return null;
            }

            doneCallback?.Invoke();
        }

        public void Reload()
        {
            Settings.TitleScreenMusic.Fade(this, 0f, Settings.FadeOutDuration, true);
            Settings.AmbienceMusic.Fade(this, 0f, Settings.FadeOutDuration, true);
            FadeOut(Settings.FadeOutDuration, () => SceneManager.LoadScene("Main"));
        }

        IEnumerator SecretEndingRoutine()
        {
            Player firstToStandDown = UnityEngine.Random.value >= .5f ? _leftPlayer : _rightPlayer;
            Player secondToStandDown = firstToStandDown == _leftPlayer ? _rightPlayer : _leftPlayer;
            firstToStandDown.StandDown();
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.33f, 1f));
            secondToStandDown.StandDown();

            yield return new WaitForSecondsRealtime(1f);

            Player firstToWalkaway = UnityEngine.Random.value >= .5f ? _leftPlayer : _rightPlayer;
            Player secondToWalkaway = firstToWalkaway == _leftPlayer ? _rightPlayer : _leftPlayer;

            firstToWalkaway.WalkAway();
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.33f, 1f));
            secondToWalkaway.WalkAway();

            yield return new WaitForSecondsRealtime(3.5f);
            FadeInText(_titleText, 2f, null);
        }

        [Conditional(Log.EDITOR_DEFINE)]
        [Conditional(Log.DEVBUILD_DEFINE)]
        void DebugStuff()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Reload();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _cameraShaker.ShakeObject(Camera.main.transform, Settings.CameraShakeShot);
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _cameraShaker.ShakeObject(Camera.main.transform, Settings.CameraShakeReloadSuccess);
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _cameraShaker.ShakeObject(Camera.main.transform, Settings.CameraShakeReloadFail);
            }
        }
    }
}
