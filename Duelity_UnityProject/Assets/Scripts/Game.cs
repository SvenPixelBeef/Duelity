using Duelity.Utility;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
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

        [SerializeField] Image _fadeOutImage;


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
            Settings.TitleScreenMusic.Play(target: this);
            Time.timeScale = 1f;
            _fadeOutImage.color = Color.black;
            _fadeOutImage.CrossFadeAlpha(0f, Settings.FadeInDuration, true);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            // TODO: Show any key prompt

            yield return new WaitUntil(() => Input.anyKeyDown);//|| Application.isEditor
                                                               // TODO: Hide any key prompt and maybe other stuff from title scene

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


        void CrossFade(float alpha, float duration, Action callback = null)
        {
            if (_crossFadeCoroutine != null)
            {
                StopCoroutine(_crossFadeCoroutine);
                _crossFadeCoroutine = null;
            }

            _crossFadeCoroutine = StartCoroutine(CrossFadeRoutine());
            IEnumerator CrossFadeRoutine()
            {
                _fadeOutImage.CrossFadeAlpha(alpha, duration, true);
                yield return new WaitForSecondsRealtime(duration);
                callback?.Invoke();
            }
        }

        public void Reload()
        {
            Settings.TitleScreenMusic.Fade(this, 0f, Settings.FadeOutDuration, true);
            Settings.AmbienceMusic.Fade(this, 0f, Settings.FadeOutDuration, true);
            CrossFade(1, Settings.FadeOutDuration, () => SceneManager.LoadScene("Main"));
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
