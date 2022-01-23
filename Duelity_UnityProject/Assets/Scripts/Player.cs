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

        public KeyCode ActionKey => _playerType == PlayerType.LeftPlayer
             ? Game.Settings.KeyCodeLeftPlayer
             : Game.Settings.KeyCodeRightPlayer;

        static readonly int parameterIdState = Animator.StringToHash("State");
        static readonly int parameterIdStateOffset = Animator.StringToHash("IdleOffset");

        const int ANIM_IDLE = 0;
        const int ANIM_FUMBLE = 1;
        const int ANIM_SHOOT = 2;
        const int ANIM_DIE = 3;
        const int ANIM_WALK = 4;
        const int ANIM_WALK_WITHOUT_GUN = 5;
        const int ANIM_IDLE_WITHOUT_GUN = 6;

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
                    Game.CameraShaker.ShakeObject(_duelMiniGameDisplay.transform, Game.Settings.CameraShakeReloadSuccess);
                    Game.Settings.ReloadSounds.RandomElement().Play();
                    if (_duelMiniGame.TargetRangeIndices.Count == 0)
                    {
                        Events.PlayerReloadedAll.RaiseEvent(this);
                    }
                }
                else
                {
                    Game.Settings.PlayerFail.Play();
                    Game.CameraShaker.ShakeObject(_duelMiniGameDisplay.transform, Game.Settings.CameraShakeReloadFail);
                    int indexOfMissedElement = _duelMiniGame.GetIndexForValue(_duelMiniGame.Value);
                    _duelMiniGameDisplay.UpdateSingleSlot(indexOfMissedElement, false);
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
        }

        void OnPlayerFailedReload(Player player)
        {
            _duelMiniGame = null;
        }

        void OnSecretEndingTriggered(Events.NoEventArgs _)
        {
            _duelMiniGame = null;
        }

        public void Shoot()
        {
            _animator.SetInteger(parameterIdState, ANIM_SHOOT);
            _shootingParticleSystem.Play();
            Game.Settings.GunShotSound.Play();
            Game.CameraShaker.ShakeObject(Camera.main.transform, Game.Settings.CameraShakeShot);
            this.WaitAndDo(new WaitForSecondsRealtime(Game.Settings.BirdReactionDelay), () =>
            {
                Game.Settings.CrowsSound.Play();
                Game.BirdsPS.Play();
            });
        }

        public void Die()
        {
            _animator.SetInteger(parameterIdState, ANIM_DIE);
        }

        bool _fumbled;

        public bool Fumbled => _fumbled;

        public void Fumble()
        {
            _animator.SetInteger(parameterIdState, ANIM_FUMBLE);
            _fumbled = true;
            StartCoroutine(CheckAnimRoutine());
            IEnumerator CheckAnimRoutine()
            {
                yield return null;
                yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
                _animator.SetInteger(parameterIdState, ANIM_IDLE_WITHOUT_GUN);
            }
        }

        public void StandDown()
        {

        }

        public void WalkAway()
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
            _animator.SetInteger(parameterIdState, _fumbled ? ANIM_WALK_WITHOUT_GUN : ANIM_WALK);
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
