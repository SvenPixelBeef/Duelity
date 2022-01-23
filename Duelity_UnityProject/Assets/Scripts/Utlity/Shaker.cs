using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Duelity
{
    public class Shaker : MonoBehaviour
    {
        readonly struct ShakeData
        {
            public Coroutine Coroutine { get; }
            public Vector3 OriginalPosition { get; }
            public Quaternion OriginalRotation { get; }

            public ShakeData(Coroutine coroutine, Transform transform)
                : this()
            {
                Coroutine = coroutine;
                OriginalPosition = transform.position;
                OriginalRotation = transform.rotation;
            }

            public void ResetTransform(Transform transform)
            {
                transform.position = OriginalPosition;
                transform.rotation = OriginalRotation;
            }
        }

        Dictionary<int, ShakeData> _activeShakes;

        void Awake()
        {
            _activeShakes = new Dictionary<int, ShakeData>();
        }

        public void ShakeObject(Transform transform, CameraShake cameraShake)
        {
            if (cameraShake == null)
                return;

            int id = transform.gameObject.GetInstanceID();
            if (_activeShakes.TryGetValue(id, out var shakeData))
            {
                StopCoroutine(shakeData.Coroutine);
                shakeData.ResetTransform(transform);
                _activeShakes.Remove(id);
            }

            ShakeData newShakeData = new ShakeData(StartCoroutine(CameraShakeRoutine(cameraShake)), transform);
            _activeShakes.Add(id, newShakeData);
            IEnumerator CameraShakeRoutine(CameraShake _cameraShake)
            {
                var currentCameraShakeTimer = 0f;
                var startingPosition = transform.position;
                var startingRotation = transform.rotation;

                float duration = _cameraShake.Duration;
                while (currentCameraShakeTimer < duration)
                {
                    float progress = currentCameraShakeTimer / duration;
                    if (_cameraShake.IntensityX != 0 || _cameraShake.IntensityY != 0)
                    {
                        float x = startingPosition.x + ((_cameraShake.GetIntensityXByProgress(progress) * _cameraShake.IntensityRandomizationX));
                        float y = startingPosition.y + ((_cameraShake.GetIntensityYByProgress(progress) * _cameraShake.IntensityRandomizationY));
                        transform.position = new Vector3(x, y, startingPosition.z);
                    }

                    if (_cameraShake.RotateCamera)
                    {
                        float intensity = (_cameraShake.GetRotationIntensityByProgress(progress) * _cameraShake.IntensityRandomizationRotation);
                        transform.rotation = Quaternion.Euler(intensity,
                                          intensity,
                                          intensity);
                    }

                    currentCameraShakeTimer += Time.unscaledDeltaTime;
                    yield return null;
                }

                transform.position = startingPosition;
                transform.rotation = startingRotation;
                _activeShakes.Remove(id);
            }
        }
    }
}
