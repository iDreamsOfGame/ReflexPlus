using System.Collections.Generic;
using ReflexPlus.Attributes;
using UnityEngine;

namespace ReflexPlus.EditorTests
{
    public class InjectedGameObject : MonoBehaviour
    {
        public readonly List<string> ExecutionOrder = new();

        [Inject]
        private void Inject()
        {
            ExecutionOrder.Add(nameof(Inject));
        }

        private void Awake()
        {
            ExecutionOrder.Add(nameof(Awake));
        }

        private void Start()
        {
            ExecutionOrder.Add(nameof(Start));
        }
    }
}