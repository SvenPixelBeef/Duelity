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

        [SerializeField] Player _leftPlayer;

        [SerializeField] Player _rightPlayer;

        [SerializeField] Image _fadeOutImage;

        Coroutine _crossFadeCoroutine;

        Coroutine _endingCoroutine;

        bool _anyEndingWasTriggered;

        public static Game Instance { get; private set; }

        public static Settings Settings => Instance._settings;


        void Awake()
        {
            Instance = this;
            this.ListenTo(Events.PlayerReloadedAll, OnPlayerReloadedAll);
            this.ListenTo(Events.PlayerFailedReload, OnPlayerFailedReload);
        }

        IEnumerator Start()
        {
            Time.timeScale = 1f;
            _fadeOutImage.color = Color.black;
            _fadeOutImage.CrossFadeAlpha(0f, Settings.FadeInDuration, true);
            yield return new WaitForSecondsRealtime(Settings.FadeInDuration);

            // TODO: Show any key prompt

            yield return new WaitUntil(() => Input.anyKeyDown || Application.isEditor);
            // TODO: Hide any key prompt and maybe other stuff from title scene


            float duelTriggerTime = Settings.DualStartTimeRange.GetRandomValueInsideRange();
            yield return new WaitForSecondsRealtime(duelTriggerTime);

            Time.timeScale = 0.1f;
            Events.DuelStarted.RaiseEvent(Events.NoArgs);

            yield return new WaitForSecondsRealtime(30f);
            if (!_anyEndingWasTriggered)
            {
                _anyEndingWasTriggered = true;
                _endingCoroutine = StartCoroutine(SecretEndingRoutine());
                IEnumerator SecretEndingRoutine()
                {
                    // TODO: do special easter egg ending here: both walk away
                    yield return null;

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

                yield return null;
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
