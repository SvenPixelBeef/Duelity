using Duelity.Utility;
using UnityEngine;

namespace Duelity
{
    public class DuelMiniGameDisplay : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _mainSpriteRenderer;

        [SerializeField] Transform _indicator;


        DuelMiniGame _duelMiniGame;

        void Awake()
        {
            _mainSpriteRenderer.gameObject.SetActive(false);
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

            foreach (var range in _duelMiniGame.ValidFloatRanges)
            {
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
        }

        void UpdateAngle()
        {
            float value = Mathf.Lerp(0f, 360f, _duelMiniGame.Value);
            _indicator.rotation = Quaternion.Euler(0, 0, value - 90f);
        }
    }
}
