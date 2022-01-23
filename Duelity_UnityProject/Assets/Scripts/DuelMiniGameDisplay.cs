using Duelity.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Duelity
{
    public class DuelMiniGameDisplay : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _mainSpriteRenderer;

        [SerializeField] Transform _indicator;

        [SerializeField] Transform _maskParent;

        List<SpriteRenderer> _spriteRenderers;


        DuelMiniGame _duelMiniGame;

        void Awake()
        {
            _mainSpriteRenderer.gameObject.SetActive(false);
            _spriteRenderers = new List<SpriteRenderer>(_maskParent.childCount);
            for (int i = 0; i < _maskParent.childCount; i++)
            {
                _spriteRenderers.Add(_maskParent.GetChild(i).GetComponent<SpriteRenderer>());
            }
        }

        void Update()
        {
            if (_duelMiniGame == null)
                return;

            UpdateAngle();
        }

        void OnDrawGizmos()
        {
            if (_duelMiniGame == null)
                return;

            for (int i = 0; i < _duelMiniGame.FloatRanges.Count; i++)
            {
                FloatRange range = _duelMiniGame.FloatRanges[i];
                if (!_duelMiniGame.TargetRangeIndices.Contains(i))
                    continue;

                float minAngle = Mathf.Lerp(0f, 360f, range.Min);
                float maxAngle = Mathf.Lerp(0f, 360f, range.Max);

                float radius = .5f;
                Vector2 center = transform.position;
                Vector2 pos1 = MathHelper.GetPointOnCircle(center, radius, minAngle * Mathf.Deg2Rad);
                Vector2 pos2 = MathHelper.GetPointOnCircle(center, radius, maxAngle * Mathf.Deg2Rad);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(center, pos1);
                Gizmos.DrawLine(center, pos2);
                Gizmos.DrawLine(pos1, pos2);
            }
        }

        public void AssignDuelMiniGame(DuelMiniGame duelMiniGame)
        {
            _duelMiniGame = duelMiniGame;
            _mainSpriteRenderer.gameObject.SetActive(true);
            UpdateAngle();
            UpdateReloadSlots();
        }

        void UpdateAngle()
        {
            float value = Mathf.Lerp(0f, 360f, _duelMiniGame.Value);
            _indicator.rotation = Quaternion.Euler(0, 0, value - 90f);
        }

        public void UpdateSingleSlot(int index)
        {
            SpriteRenderer toUpdate = _spriteRenderers[index];
            Game.Instance.FadeInSpriteRenderer(toUpdate, .66f, AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));
        }

        public void UpdateReloadSlots()
        {
            foreach (var renderer in _spriteRenderers)
            {
                var color = renderer.color;
                color.a = 1f;
                renderer.color = color;
            }

            foreach (var index in _duelMiniGame.TargetRangeIndices)
            {
                var color = _spriteRenderers[index].color;
                color.a = 0f;
                _spriteRenderers[index].color = color;
            }
        }
    }
}
