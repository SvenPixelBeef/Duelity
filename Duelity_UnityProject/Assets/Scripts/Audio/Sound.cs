using UnityEngine;
using UnityEngine.Audio;

namespace Duelity
{
    [CreateAssetMenu(fileName = "Sound", menuName = "Duelity/Sound")]
    public class Sound : ScriptableObject
    {
        [SerializeField]
        AudioClip clip = null;
        public AudioClip Clip => clip;

        [SerializeField]
        AudioMixerGroup audioMixerGroup = null;
        public AudioMixerGroup AudioMixerGroup => audioMixerGroup;

        [SerializeField]
        [Range(0f, 1f)]
        float volume = 1f;
        public float Volume => volume;

        [SerializeField]
        [Range(-3f, 3f)]
        float pitch = 1f;
        public float Pitch => pitch;

        [SerializeField]
        bool loop = false;
        public bool Loop => loop;
    }
}