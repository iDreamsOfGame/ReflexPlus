using UnityEngine;
using UnityEngine.Serialization;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class PickupSoundEffect : MonoBehaviour
    {
        [FormerlySerializedAs("_lifeTime")]
        [SerializeField, Min(1f)]
        private float lifeTime;

        [FormerlySerializedAs("_pitchVariation")]
        [SerializeField, Min(0.1f)]
        private float pitchVariation;

        [FormerlySerializedAs("_audioSource")]
        [SerializeField]
        private AudioSource audioSource;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
            audioSource.pitch += Random.Range(-pitchVariation, +pitchVariation);
            audioSource.Play();
        }
    }
}