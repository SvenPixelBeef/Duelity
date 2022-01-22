using Duelity.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Diagnostics;

namespace Duelity
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Expandable] Settings _settings;

        [SerializeField] Image _fadeOutImage;

        Coroutine _crossFadeRoutine;

        [SerializeField] bool _anyKeyWasDown;

        float _duelTriggerTime;
        float _duelTriggerTimer;
        bool _duelTriggered;

        public static Game Instance { get; private set; }

        public static Settings Settings => Instance._settings;


        void Awake()
        {
            Instance = this;
            _duelTriggerTime = Settings.DualStartTimeRange.GetRandomValueInsideRange();
        }

        void Start()
        {
            Time.timeScale = 1f;
            _fadeOutImage.color = Color.black;
            _fadeOutImage.CrossFadeAlpha(0f, 1f, true);
        }

        void Update()
        {
            DebugReloadScene();

            if (!_anyKeyWasDown)
            {
                _anyKeyWasDown = Input.anyKeyDown;
                if (!_anyKeyWasDown)
                    return;
            }

            if (_duelTriggered)
                return;

            _duelTriggerTimer += Time.deltaTime;
            if (_duelTriggerTimer >= _duelTriggerTime)
            {
                Time.timeScale = 0.1f;
                Events.DuelStarted.RaiseEvent(Events.NoArgs);
                _duelTriggered = true;
            }
        }

        public void Reload()
        {
            CrossFade(1, 1f, () => SceneManager.LoadScene("Main"));
        }

        void CrossFade(float alpha, float duration, Action callback = null)
        {
            if (_crossFadeRoutine != null)
            {
                StopCoroutine(_crossFadeRoutine);
                _crossFadeRoutine = null;
            }

            _crossFadeRoutine = StartCoroutine(CrossFadeRoutine());
            IEnumerator CrossFadeRoutine()
            {
                _fadeOutImage.CrossFadeAlpha(alpha, duration, true);
                yield return new WaitForSecondsRealtime(duration);
                callback?.Invoke();
            }
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
