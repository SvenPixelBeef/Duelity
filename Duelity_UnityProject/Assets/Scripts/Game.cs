using Duelity.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duelity
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Expandable] Settings _settings;


        float _duelTriggerTime;
        float _duelTriggerTimer;
        bool _duelTriggered;


        public static Game Instance { get; private set; }

        public static Settings Settings => Instance._settings;


        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            _duelTriggerTime = Settings.DualStartTimeRange.GetRandomValueInsideRange();
        }

        void Start()
        {
            Time.timeScale = 1f;
        }

        void Update()
        {

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
    }
}
