using UnityEngine;

namespace Duelity
{
    [CreateAssetMenu(menuName = "CameraShake", fileName = "Duelity/CameraShake")]
    public class CameraShake : ScriptableObject
    {
        [Range(0f, 2f)] [SerializeField] private float duration;
        public float Duration => duration;

        [SerializeField] private AnimationCurve animationCurveX;
        public AnimationCurve AnimationCurveX => animationCurveX;

        [SerializeField] private AnimationCurve animationCurveY;
        public AnimationCurve AnimationCurveY => animationCurveY;

        [Range(0f, 5f)] [SerializeField] private float intensityX = 1f;
        public float IntensityX
        {
            get
            {
                int sign = 1;
                if (RandomizeDirection)
                    sign = UnityEngine.Random.value > .5f ? 1 : -1;
                float intensity = intensityX * sign;
                if (RandomizeIntensityX)
                {
                    intensity = 0f;
                    intensity = Mathf.Clamp(
                        value: intensity + UnityEngine.Random.Range(RandomizedXCurve.constantMin, RandomizedXCurve.constantMax),
                        min: MinIntensityX,
                        max: float.MaxValue);
                    return intensity;
                }
                return intensity;
            }
        }

        public float GetIntensityXByProgress(float progress) => AnimationCurveX.Evaluate(progress) * IntensityX;

        [Range(10f, 50f)] [SerializeField] private float frequencyX = 15f;
        public float FrequencyX => frequencyX;

        [SerializeField] private bool randomizeIntensityX = false;
        public bool RandomizeIntensityX => randomizeIntensityX;

        [SerializeField] private ParticleSystem.MinMaxCurve randomizedXCurve;
        public ParticleSystem.MinMaxCurve RandomizedXCurve => randomizedXCurve;

        [Range(0f, 2f)] [SerializeField] private float minIntensityX = 0f;
        public float MinIntensityX => minIntensityX;

        public float IntensityRandomizationX => (Mathf.PerlinNoise(78312.75412f, Time.time * FrequencyX) * 2 - 1);



        [Range(0f, 5f)] [SerializeField] private float intensityY = 1f;
        public float IntensityY
        {
            get
            {
                int sign = 1;
                if (RandomizeDirection)
                    sign = UnityEngine.Random.value > .5f ? 1 : -1;
                float intensity = intensityY * sign;
                if (RandomizeIntensityY)
                {
                    intensity = 0f;
                    intensity = Mathf.Clamp(
                        value: intensity + UnityEngine.Random.Range(RandomizedYCurve.constantMin, RandomizedYCurve.constantMax),
                        min: MinIntensityY,
                        max: float.MaxValue);
                    return intensity;
                }
                return intensity;
            }
        }

        public float GetIntensityYByProgress(float progress) => AnimationCurveY.Evaluate(progress) * IntensityY;


        [Range(1f, 50f)] [SerializeField] private float frequencyY = 15f;
        public float FrequencyY => frequencyY;

        [SerializeField] private bool randomizeIntensityY = false;
        public bool RandomizeIntensityY => randomizeIntensityY;

        [SerializeField] private ParticleSystem.MinMaxCurve randomizedYCurve;
        public ParticleSystem.MinMaxCurve RandomizedYCurve => randomizedYCurve;

        [Range(0f, 2f)] [SerializeField] private float minIntensityY = 0f;
        public float MinIntensityY => minIntensityY;
        public float IntensityRandomizationY => (Mathf.PerlinNoise(13781.45754f, Time.time * FrequencyY) * 2 - 1);



        [SerializeField] private bool randomizeDirection = true;
        public bool RandomizeDirection => randomizeDirection;

        [Header("Rotation")]

        [SerializeField] private bool rotateCamera = false;
        public bool RotateCamera => rotateCamera;

        [SerializeField] private AnimationCurve rotationAnimationCurve;
        public AnimationCurve RotationAnimationCurve => rotationAnimationCurve;

        [Range(0f, 10f)] [SerializeField] private float rotationIntensity = 5f;
        public float RotationIntensity
        {
            get
            {
                if (RandomizeDirection)
                    return rotationIntensity * Random.value > .5f ? 1 : -1;
                return rotationIntensity;
            }
        }

        public float GetRotationIntensityByProgress(float progress) => RotationAnimationCurve.Evaluate(progress) * RotationIntensity;


        [Range(1f, 50f)] [SerializeField] private float rotationFrequency = 15f;
        public float RotationFrequency => rotationFrequency;

        public float IntensityRandomizationRotation => (Mathf.PerlinNoise(16912.2456f, Time.time * RotationFrequency) * 2 - 1);

    }
}