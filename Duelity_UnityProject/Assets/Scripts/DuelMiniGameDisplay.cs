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

        List<SpriteMask> _masks;


        DuelMiniGame _duelMiniGame;

        void Awake()
        {
            _mainSpriteRenderer.gameObject.SetActive(false);
            _masks = new List<SpriteMask>(_maskParent.childCount);
            for (int i = 0; i < _maskParent.childCount; i++)
            {
                _masks.Add(_maskParent.GetChild(i).GetComponent<SpriteMask>());
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

        public void UpdateReloadSlots()
        {
            foreach (var mask in _masks)
            {
                mask.enabled = true;
            }

            foreach (var index in _duelMiniGame.TargetRangeIndices)
            {
                _masks[index].enabled = false;
            }
        }
    }
}
