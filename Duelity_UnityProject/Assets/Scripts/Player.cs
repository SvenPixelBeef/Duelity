using Duelity.Utility;
using System;
using System.Collections;
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

        [SerializeField] ParticleSystem _shootingParticleSystem;

        DuelMiniGame _duelMiniGame;

        KeyCode ActionKey => _playerType == PlayerType.LeftPlayer
            ? Game.Settings.KeyCodeLeftPlayer
            : Game.Settings.KeyCodeRightPlayer;

        static readonly int parameterIdState = Animator.StringToHash("State");
        static readonly int parameterIdStateOffset = Animator.StringToHash("IdleOffset");

        const int ANIM_IDLE = 0;
        const int ANIM_DRAW_WEAPON = 1;
        const int ANIM_SHOOT = 2;
        const int ANIM_DIE = 3;
        const int ANIM_WALK = 4;
        const int ANIM_PUT_WEAPON_AWAY = 5;

        void Awake()
        {
            this.ListenTo(Events.DuelStarted, OnDuelStarted);
            this.ListenTo(Events.PlayerReloadedAll, OnPlayerReloadedAll);
            this.ListenTo(Events.PlayerFailedReload, OnPlayerFailedReload);
            this.ListenTo(Events.SecretEndingTriggered, OnSecretEndingTriggered);
            _animator.SetFloat(parameterIdStateOffset, UnityEngine.Random.value);
        }

        void OnDestroy()
        {
            this.StopListeningTo(Events.DuelStarted);
            this.StopListeningTo(Events.PlayerReloadedAll);
            this.StopListeningTo(Events.PlayerFailedReload);
            this.StopListeningTo(Events.SecretEndingTriggered);
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
                    _duelMiniGame.RemoveRange(hitRange, out int removedIndex);
                    _duelMiniGameDisplay.UpdateSingleSlot(removedIndex);
                    if (_duelMiniGame.TargetRangeIndices.Count == 0)
                    {
                        Events.PlayerReloadedAll.RaiseEvent(this);
                    }
                    else
                    {
                        Game.Settings.ReloadSounds.RandomElement().Play();
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
            _animator.SetInteger(parameterIdState, ANIM_DRAW_WEAPON);
        }

        void OnPlayerReloadedAll(Player player)
        {
            _duelMiniGame = null;
            //_duelMiniGameDisplay.gameObject.SetActive(false);
            if (player == this)
            {
                Log.Info($"{_playerType} won!");
            }
        }

        void OnPlayerFailedReload(Player player)
        {
            _duelMiniGame = null;
            // _duelMiniGameDisplay.gameObject.SetActive(false);
            if (player == this)
            {
                Log.Info($"{_playerType} failed to reload!");
            }
        }

        void OnSecretEndingTriggered(Events.NoEventArgs _)
        {
            _duelMiniGame = null;
            //_duelMiniGameDisplay.gameObject.SetActive(false);
        }

        public void Shoot()
        {
            _animator.SetInteger(parameterIdState, ANIM_SHOOT);
            _shootingParticleSystem.Play();
            Game.Settings.GunShotSound.Play();
            this.WaitAndDo(new WaitForSecondsRealtime(UnityEngine.Random.Range(0.75f, 1.25f)), () =>
            {
                Game.Settings.CrowsSound.Play();
                // TODO: Crow particle effect play here

            });
        }

        public void Die()
        {
            _animator.SetInteger(parameterIdState, ANIM_DIE);

            // TODO:Sound effect for dying goes here

        }

        public void StandDown()
        {
            _animator.SetInteger(parameterIdState, ANIM_PUT_WEAPON_AWAY);
        }

        public void WalkAway()
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
            _animator.SetInteger(parameterIdState, ANIM_WALK);
            StartCoroutine(WalkAwayRoutine());
            IEnumerator WalkAwayRoutine()
            {
                Vector2 dir = _playerType == PlayerType.LeftPlayer ? Vector2.left : Vector2.right;
                float duration = 10f;
                float elapsedTime = 0f;
                while (elapsedTime < duration)
                {
                    transform.Translate(dir * Game.Settings.WalkAwaySpeed * Time.unscaledDeltaTime);
                    elapsedTime += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
        }
    }
}
