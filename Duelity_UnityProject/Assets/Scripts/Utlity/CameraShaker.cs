using UnityEngine;
using System.Collections;

namespace Duelity
{
    public class CameraShaker : MonoBehaviour
    {
        CameraShake currentCameraShake;
        Coroutine currentCameraShakeCoroutine;

        Vector3 cameraStartingPosition;
        float currentCameraShakeTimer;

        public void ShakeCamera(CameraShake cameraShake, float? overrideDuration = null)
        {
            if (!cameraShake)
                return;

            StopAnyOngoingShake();
            currentCameraShake = cameraShake;
            currentCameraShakeCoroutine = StartCoroutine(CameraShakeRoutine(currentCameraShake));

            IEnumerator CameraShakeRoutine(CameraShake _cameraShake)
            {
                currentCameraShakeTimer = 0f;
                cameraStartingPosition = transform.position;

                float duration = overrideDuration.HasValue ? overrideDuration.Value : _cameraShake.Duration;
                while (currentCameraShakeTimer < duration)
                {
                    float progress = currentCameraShakeTimer / duration;
                    if (_cameraShake.IntensityX != 0 || _cameraShake.IntensityY != 0)
                    {
                        float x = cameraStartingPosition.x + ((_cameraShake.GetIntensityXByProgress(progress) * _cameraShake.IntensityRandomizationX));
                        float y = cameraStartingPosition.y + ((_cameraShake.GetIntensityYByProgress(progress) * _cameraShake.IntensityRandomizationY));
                        transform.position = new Vector3(x, y, cameraStartingPosition.z);
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

                transform.position = cameraStartingPosition;
                transform.rotation = Quaternion.identity;
                currentCameraShakeCoroutine = null;
            }
        }

        public void StopAnyOngoingShake()
        {
            if (currentCameraShakeCoroutine != null)
            {
                StopCoroutine(currentCameraShakeCoroutine);
                transform.position = cameraStartingPosition;
                transform.rotation = Quaternion.identity;
                currentCameraShakeCoroutine = null;
            }
        }
    }
}
