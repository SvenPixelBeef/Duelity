using UnityEngine;

namespace Duelity.Utility
{
    public static class Coroutiner
    {
        public class CoroutineHelper : MonoBehaviour { }

        static CoroutineHelper _instance;

        public static CoroutineHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameObject = new GameObject("Coroutiner");
                    _instance = gameObject.AddComponent<CoroutineHelper>();
                    gameObject.hideFlags = HideFlags.HideInHierarchy;
                }

                return _instance;
            }
        }
    }
}
