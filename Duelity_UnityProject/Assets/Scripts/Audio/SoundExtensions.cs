using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System;
using Duelity.Utility;

namespace Duelity
{
    public static class SoundExtensions
    {
        public static void Play(this Sound sound)
        {
            Game.Audio.PlaySound(sound);
        }

        public static void Play(this Sound sound, float volumeOverride, float pitchOverride)
        {
            Game.Audio.PlaySound(sound, volumeOverride, pitchOverride);
        }

        public static void Play(this Sound sound, object target)
        {
            Game.Audio.PlaySound(sound, target);
        }

        public static void Play(this Sound sound, object target, float volumeOverride, float pitchOverride)
        {
            Game.Audio.PlaySound(sound, target, volumeOverride, pitchOverride);
        }

        public static void Stop(this Sound sound, object target)
        {
            Game.Audio.StopSound(sound, target);
        }

        public static void StopAllSounds(this object target)
        {
            Game.Audio.StopAllSoundsOnTarget(target);
        }

        public static void Fade(this Sound sound, object target, float targetVolume, float duration, bool ignoreTimescale = false)
        {
            Game.Audio.FadeSound(sound, target, targetVolume, duration, ignoreTimescale);
        }

        public static void ApplySoundSettings(this AudioSource audioSource, Sound sound)
        {
            audioSource.ApplySoundSettings(sound.Clip, sound.Volume, sound.Pitch, sound.Loop, sound.AudioMixerGroup);
        }

        public static void ApplySoundSettings(this AudioSource audioSource, Sound sound, float overrideVolume, float overridePitch)
        {
            audioSource.ApplySoundSettings(sound.Clip, overrideVolume, overridePitch, sound.Loop, sound.AudioMixerGroup);
        }

        public static void ApplySoundSettings(this AudioSource audioSource,
            AudioClip clip,
            float volume,
            float pitch,
            bool loop,
            AudioMixerGroup audioMixerGroup)
        {
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }


        static readonly Func<float> DeltaTimeFunc = () => Time.deltaTime;

        static readonly Func<float> IgnoreTimeScaleTimeFunc = () => Time.unscaledDeltaTime;

        public static void LerpVolume(this AudioSource audioSource, float targetVolume, float duration, bool ignoreTimeScale = false)
        {
            Debug.Assert(duration > 0);
            Debug.Assert(targetVolume >= 0f && targetVolume <= 1f);
            Coroutiner.Instance.StartCoroutine(LerpVolumeRoutine(
                audioSource_: audioSource,
                targetVolume_: targetVolume,
                duration_: duration,
                timeFunc: ignoreTimeScale ? IgnoreTimeScaleTimeFunc : DeltaTimeFunc));

            IEnumerator LerpVolumeRoutine(AudioSource audioSource_, float targetVolume_, float duration_, Func<float> timeFunc)
            {
                float elapsedTime = 0f;
                float startingVolume = audioSource_.volume;
                while (elapsedTime < duration_)
                {
                    audioSource_.volume = Mathf.Lerp(startingVolume, targetVolume_, elapsedTime / duration_);
                    elapsedTime += timeFunc();
                    yield return null;
                }
            }
        }
    }
}