using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Duelity
{
    public class AudioManager
    {
        Dictionary<int, Dictionary<Sound, List<AudioSource>>> _sounds;

        GameObject _audioSourceHolder;

        List<AudioSource> _pooledSources;

        public AudioManager(int maxSources)
        {
            _sounds = new Dictionary<int, Dictionary<Sound, List<AudioSource>>>();
            _pooledSources = new List<AudioSource>(maxSources);

            _audioSourceHolder = new GameObject("AudioSources");
            GameObject.DontDestroyOnLoad(_audioSourceHolder);
            for (int i = 0; i < maxSources; i++)
            {
                var source = _audioSourceHolder.AddComponent<AudioSource>();
                source.playOnAwake = false;
                _pooledSources.Add(source);

            }
        }

        #region OneShot

        public void PlaySound(Sound sound)
        {
            PlaySoundInternal(sound);
        }

        public void PlaySound(Sound sound, float overrideVolume, float overridePitch)
        {
            PlaySoundInternal(sound, overrideVolume, overridePitch);
        }

        void PlaySoundInternal(Sound sound, float? overrideVolume = null, float? overridePitch = null)
        {
            float volume = overrideVolume.HasValue ? overrideVolume.Value : sound.Volume;
            float pitch = overridePitch.HasValue ? overridePitch.Value : sound.Pitch;

            var source = GetAudioSource();
            source.ApplySoundSettings(sound, volume, pitch);
            source.Play();
        }

        #endregion

        public void PlaySound(Sound sound, object target)
        {
            PlaySoundInternal(sound, target);
        }

        public void PlaySound(Sound sound, object target, float volumeOverride, float pitchOverride)
        {
            PlaySoundInternal(sound, target, volumeOverride, pitchOverride);
        }

        void PlaySoundInternal(Sound sound, object target, float? overrideVolume = null, float? overridePitch = null)
        {
            UpdatePlayingSounds();

            AudioSource audioSource = GetAudioSource();

            float volume = overrideVolume.HasValue ? overrideVolume.Value : sound.Volume;
            float pitch = overridePitch.HasValue ? overridePitch.Value : sound.Pitch;
            audioSource.ApplySoundSettings(sound, volume, pitch);

            Dictionary<Sound, List<AudioSource>> targetSounds;
            bool targetExists = _sounds.TryGetValue(target.GetHashCode(), out targetSounds);
            if (!targetExists)
            {
                targetSounds = new Dictionary<Sound, List<AudioSource>>();
                _sounds.Add(target.GetHashCode(), targetSounds);
            }

            List<AudioSource> sources;
            bool sameSoundAlreadyPlaying = targetSounds.TryGetValue(sound, out sources);
            if (!sameSoundAlreadyPlaying)
            {
                sources = new List<AudioSource>();
                targetSounds.Add(sound, sources);
            }

            sources.Add(audioSource);
            audioSource.Play();
        }

        public void StopSound(Sound sound, object target)
        {
            bool targetExists = _sounds.TryGetValue(target.GetHashCode(), out var targetSounds);
            if (!targetExists)
                return;

            bool soundPlaying = targetSounds.TryGetValue(sound, out var sources);
            if (!soundPlaying)
                return;

            foreach (AudioSource source in sources)
                source.Stop();

            targetSounds.Remove(sound);
        }

        public void StopAllSoundsOnTarget(object target)
        {
            bool targetExists = _sounds.TryGetValue(target.GetHashCode(), out var targetSounds);
            if (!targetExists)
                return;

            foreach (KeyValuePair<Sound, List<AudioSource>> item in targetSounds)
            {
                foreach (AudioSource source in item.Value)
                    source.Stop();
            }

            targetSounds.Clear();
        }

        public void StopAllSounds()
        {
            foreach (Dictionary<Sound, List<AudioSource>> dict in _sounds.Values)
            {
                foreach (var list in dict.Values)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (list[i].isPlaying)
                        {
                            list[i].Stop();
                            list.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void FadeSound(Sound sound, object target, float targetVolume, float duration, bool ignoreTimescale = false)
        {
            bool targetExists = _sounds.TryGetValue(target.GetHashCode(), out var targetSounds);
            if (!targetExists)
                return;

            bool soundPlaying = targetSounds.TryGetValue(sound, out var sources);
            if (!soundPlaying)
                return;

            foreach (AudioSource source in sources)
                source.LerpVolume(targetVolume, duration, ignoreTimescale);
        }

        public void Cleanup()
        {
            _sounds.Clear();
            _pooledSources.Clear();
            GameObject.Destroy(_audioSourceHolder);
        }

        void UpdatePlayingSounds()
        {
            foreach (Dictionary<Sound, List<AudioSource>> dict in _sounds.Values)
            {
                foreach (var list in dict.Values)
                {
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        if (!list[i].isPlaying)
                        {
                            list.RemoveAt(i);
                        }
                    }
                }
            }
        }

        AudioSource GetAudioSource()
        {
            var source = _pooledSources.FirstOrDefault(s => !s.isPlaying);
            Debug.Assert(source != null, "Not enough pooled audio sources available!");
            return source;
        }
    }
}