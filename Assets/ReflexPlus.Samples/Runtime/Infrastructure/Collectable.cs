using ReflexPlus.Attributes;
using ReflexPlus.Sample.Application;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class Collectable : MonoBehaviour
    {
        [FormerlySerializedAs("_id")]
        [SerializeField]
        private string id;

        [Inject]
        private ICollectionStorage collectionStorage;

        [Inject]
        private PickupSoundEffect pickupSoundEffectPrefab;

        private void Start()
        {
            gameObject.SetActive(!collectionStorage.IsCollected(id));
        }

        public void Collect()
        {
            gameObject.SetActive(false);
            collectionStorage.Add(id);
            Instantiate(pickupSoundEffectPrefab);
        }
    }
}