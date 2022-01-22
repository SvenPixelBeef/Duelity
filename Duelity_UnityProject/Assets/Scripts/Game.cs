using Duelity.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Duelity
{
    public class Game : MonoBehaviour
    {
        [SerializeField, Expandable] Settings _settings;



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
        }

        void Start()
        {
            Events.DuelStarted.RaiseEvent(Events.NoArgs);
        }
    }
}
