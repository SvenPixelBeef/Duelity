using Duelity.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Duelity
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(DuelMiniGameDisplay))]
    public class Player : MonoBehaviour
    {
        [SerializeField] PlayerType _playerType;

        [SerializeField] DuelMiniGameDisplay _duelMiniGameDisplay;

        [SerializeField] Animator _animator;

        [SerializeField] SpriteRenderer _spriteRenderer;

        DuelMiniGame _duelMiniGame;

        KeyCode ActionKey => _playerType == PlayerType.LeftPlayer
            ? Game.Settings.KeyCodeLeftPlayer
            : Game.Settings.KeyCodeRightPlayer;


        void Awake()
        {
            this.enabled = false;
            this.ListenTo(Events.DuelStarted, OnDuelStarted);
        }

        void OnDestroy()
        {
            this.StopListeningTo(Events.DuelStarted);
        }

        void Update()
        {
            _duelMiniGame.Update(Time.deltaTime);
            if (Input.GetKeyDown(ActionKey))
            {
                Log.Info($"{_playerType} pressed ActionKey {ActionKey}");
                var hit = _duelMiniGame.IsInsideValidRange(out FloatRange hitRange);
                if (hit)
                {
                    _duelMiniGame.RemoveRange(hitRange);
                    // Check for all ranges hit etc.
                }
            }
        }

        void OnDuelStarted(Events.NoEventArgs _)
        {
            _duelMiniGame = new DuelMiniGame(Game.Settings.DuelMiniGameConfig);
            _duelMiniGameDisplay.AssignDuelMiniGame(_duelMiniGame);
            this.enabled = true;
        }
    }
}
