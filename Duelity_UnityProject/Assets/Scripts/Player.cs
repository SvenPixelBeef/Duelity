using Duelity.Utility;
using UnityEngine;

namespace Duelity
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
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
            this.ListenTo(Events.DuelStarted, OnDuelStarted);
            this.ListenTo(Events.PlayerReloadedAll, OnPlayerReloadedAll);
            this.ListenTo(Events.PlayerFailedReload, OnPlayerFailedReload);
        }

        void OnDestroy()
        {
            this.StopListeningTo(Events.DuelStarted);
            this.StopListeningTo(Events.PlayerReloadedAll);
            this.StopListeningTo(Events.PlayerFailedReload);
        }

        void Update()
        {
            if (_duelMiniGame == null)
                return;

            _duelMiniGame.Update(Time.unscaledDeltaTime);
            if (Input.GetKeyDown(ActionKey))
            {
                Log.Info($"{_playerType} pressed ActionKey {ActionKey}");
                bool hit = _duelMiniGame.IsInsideValidRange(out FloatRange hitRange);
                if (hit)
                {
                    _duelMiniGame.RemoveRange(hitRange);
                    // more feedback stuff here
                    if (_duelMiniGame.ValidFloatRanges.Count == 0)
                    {
                        Events.PlayerReloadedAll.RaiseEvent(this);
                    }
                }
                else
                {
                    // Game over?
                    Events.PlayerFailedReload.RaiseEvent(this);
                }
            }
        }

        void OnDuelStarted(Events.NoEventArgs _)
        {
            _duelMiniGame = new DuelMiniGame(Game.Settings.DuelMiniGameConfig);
            _duelMiniGameDisplay.AssignDuelMiniGame(_duelMiniGame);
        }

        void OnPlayerReloadedAll(Player player)
        {
            _duelMiniGame = null;
            _duelMiniGameDisplay.gameObject.SetActive(false);
            if (player == this)
            {
                Log.Info($"{_playerType} won!");
            }
        }

        void OnPlayerFailedReload(Player player)
        {
            _duelMiniGame = null;
            _duelMiniGameDisplay.gameObject.SetActive(false);
            if (player == this)
            {
                Log.Info($"{_playerType} failed to reload!");
            }
        }
    }
}
