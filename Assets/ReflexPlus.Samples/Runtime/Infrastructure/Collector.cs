using ReflexPlus.Attributes;
using ReflexPlus.Sample.Application;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class Collector : MonoBehaviour
    {
        [Inject]
        private readonly ICollectorInput input;

        [Inject]
        private readonly CollectorConfigurationModel model;

        private void Update()
        {
            var value = this.input.Get();
            var motion = Vector3.ClampMagnitude(new Vector3(value.x, 0, value.y), 1);
            transform.Translate(motion * (Time.deltaTime * model.MovementSpeed));
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Collectable collectable))
            {
                collectable.Collect();
            }
        }
    }
}