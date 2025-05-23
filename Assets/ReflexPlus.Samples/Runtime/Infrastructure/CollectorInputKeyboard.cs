using ReflexPlus.Sample.Application;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class CollectorInputKeyboard : ICollectorInput
    {
        public Vector2 Get()
        {
            return new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
        }
    }
}