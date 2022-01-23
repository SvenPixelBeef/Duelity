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

        [SerializeField] SpriteRenderer _fadeOutSpriteRenderer;

        [SerializeField] TextMeshPro _titleText;
        [SerializeField] TextMeshPro _anyKeyPromptText;


        Color FadeOutColor => Color.black;
        Color FadeInColor => new Color(0f, 0f, 0f, 0f);

        Coroutine _crossFadeCoroutine;

        Coroutine _endingCoroutine;

        bool _anyEndingWasTriggered;

        static AudioManager _audio;

        public static Game Instance { get; private set; }

        public static Settings Settings => Instance._settings;
        public static AudioManager Audio => _audio ??= new AudioManager(10);

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
        }

        IEnumerator Start()
        {
            _anyKeyPromptText.enabled = false;
            Settings.TitleScreenMusic.Play(target: this);
            Time.timeScale = 1f;
            FadeIn(Settings.FadeInDuration);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            _anyKeyPromptText.enabled = true;

            yield return new WaitUntil(() => Input.anyKeyDown);//|| Application.isEditor

            _titleText.enabled = false;
            _anyKeyPromptText.enabled = false;

            float musicFadeDuration = 1.5f;
            Settings.AmbienceMusic.Play(this, 0f, 1f);
            Settings.AmbienceMusic.Fade(this, Settings.AmbienceMusic.Volume, musicFadeDuration, true);
            Settings.TitleScreenMusic.Fade(this, 0f, musicFadeDuration, true);
            // yield return new WaitForSecondsRealtime(musicFadeDuration);


            float duelTriggerTime = Settings.DualStartTimeRange.GetRandomValueInsideRange();
            yield return new WaitForSecondsRealtime(duelTriggerTime);

            Events.DuelStarted.RaiseEvent(Events.NoArgs);
            //   Time.timeScale = 0.1f;

            // TODO: Play more/other sounds/music here?


            yield return new WaitForSecondsRealtime(Settings.RequiredDurationSecretEnding);
            if (!_anyEndingWasTriggered)
            {
                Events.SecretEndingTriggered.RaiseEvent(Events.NoArgs);
                _anyEndingWasTriggered = true;
                _endingCoroutine = StartCoroutine(SecretEndingRoutine());
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
                }
            }
        }

        void OnDestroy()
        {
            this.StopListeningTo(Events.PlayerReloadedAll);
            this.StopListeningTo(Events.PlayerFailedReload);
        }

        void Update()
        {
            DebugReloadScene();
        }


        void OnPlayerFailedReload(Player playerWhoFailed)
        {
            Debug.Assert(_endingCoroutine == null);
            _endingCoroutine = StartCoroutine(PlayerFailedEndingRoutine());
            IEnumerator PlayerFailedEndingRoutine()
            {
                _anyEndingWasTriggered = true;

                yield return null;
            }
        }

        void OnPlayerReloadedAll(Player winningPlayer)
        {
            Debug.Assert(_endingCoroutine == null);
            _endingCoroutine = StartCoroutine(PlayerSuccededEndingRoutine());
            IEnumerator PlayerSuccededEndingRoutine()
            {
                _anyEndingWasTriggered = true;
                Time.timeScale = 1f;
                winningPlayer.Shoot();
                yield return null;
                Player losingPlayer = _leftPlayer == winningPlayer ? _rightPlayer : _leftPlayer;
                losingPlayer.Die();

                yield return new WaitForSecondsRealtime(2f);

                winningPlayer.WalkAway();
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

        IEnumerator ColorLerpRoutine(Color startingColor,
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

        [Conditional(Log.EDITOR_DEFINE)]
        [Conditional(Log.DEVBUILD_DEFINE)]
        void DebugReloadScene()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Reload();
            }
        }
    }
}
